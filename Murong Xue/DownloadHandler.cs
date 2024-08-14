using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal sealed class DownloadHandler
    {
        int BATCH_SIZE = 7;
        int BATCH_DELAY_MS = 200;
        int BATCH_MIN_TIME = 1000;
        //"fails" will be used for  batch size & delay adjustments
        private readonly object fail_lock = new();
        private int fails = 0;
        //---
        private static DownloadHandler? s_DownloadHandler = null;
        private readonly static HttpClient client = new();
        //c# version 12 does not have System.Threading.Lock, so we use Object()
        private readonly object DPLock = new(); //for when swapping between the two lists.
        //list of files to be downloaded
        private readonly List<DownloadEntryBase> Queued = [];
        //Not in download, but also not done.
        private readonly List<DownloadEntryBase> Downloading = [];
        private readonly List<DownloadEntryBase> Processing = [];
        private static Reporter report;
        
        private DownloadHandler()
        {
            report = Config.OneReporterPlease("DownloadHandler");
        }
        public static DownloadHandler GetInstance()
        {
            s_DownloadHandler ??= new DownloadHandler();
            report.Log(LogFlag.DEBUG_SPAM, "GetInstance");
            return s_DownloadHandler;
        }
        public async Task ProcessDownloads()
        {
            report.Log(LogFlag.DEBUG, "Processing Downloads");
            List<Task> CurrentBatch = [];
            report.Log(LogFlag.DEBUG, $"Before processing, Queued[{Queued.Count}] Downloading[{Downloading.Count}]");
            while (Queued.Count != 0 || Downloading.Count != 0 || Processing.Count != 0) //these are volatile, might consider some type of sentinel/semaphore to handle this behavior
            {
                while (Queued.Count != 0)
                {
                    DownloadEntryBase entry = PopSwapDownload();
                    //Add the entry to the task list
                    CurrentBatch.Add(Task.Run(() => entry.Request(client)));
                    //When we've filled our budget or used em all
                    if (CurrentBatch.Count >= BATCH_SIZE || Queued.Count == 0)
                    {
                        CurrentBatch.Add(Task.Delay(BATCH_MIN_TIME));
                        report.Log(LogFlag.DEBUG, $"\t\tQ[{Queued.Count}]  D[{Downloading.Count}]  P[{Processing.Count}]\tMIN TIME{BATCH_MIN_TIME}ms");
                        await Task.WhenAll(CurrentBatch);
                        CurrentBatch.Clear();
                    }
                }
            }
            lock(DPLock)
            {
                if (Queued.Count != 0 || Downloading.Count != 0 || Processing.Count != 0)
                {
                    report.Log(LogFlag.ERROR, "Time to use a better method for handling this loop. " +
                        "Either Queued/Downloading/Processing had a value after the while loop ended");
                }
            }
            report.Log(LogFlag.NOTEWORTHY, "Queue Complete!");
            report.Log(LogFlag.DEBUG, $"\t\tQ[{Queued.Count}]  D[{Downloading.Count}]  P[{Processing.Count}]");
        }
        public void QueueDownload(DownloadEntryBase entry)
        {
            report.Log(LogFlag.DEBUG_SPAM, "Add to Queue (waiting on lock)");
            lock(DPLock)
            {
                Queued.Add(entry);
                report.Log(LogFlag.DEBUG_SPAM, "Queued (releasing lock)");
            }
        }
        private DownloadEntryBase PopSwapDownload()
        {
            report.Log(LogFlag.DEBUG_SPAM, "PopSwap Q->D (waiting on lock)");
            DownloadEntryBase? entry = null;
            lock (DPLock)
            {
                entry = Queued.First();
                Queued.Remove(entry);
                Downloading.Add(entry);
            }
            report.Log(LogFlag.DEBUG_SPAM, "PopSwapped (released lock)");
            return entry;
        }
        public void DownloadingToProcessing(DownloadEntryBase entry)
        {
            report.Log(LogFlag.DEBUG_SPAM, "DownloadingToProcessing (Waiting on lock)");
            lock (DPLock)
            {
                Downloading.Remove(entry);
                Processing.Add(entry);
            }
            report.Log(LogFlag.DEBUG_SPAM, "DownloadingToProcessing (released lock)");
        }
        public void RemoveProcessing(DownloadEntryBase entry)
        {
            report.Log(LogFlag.DEBUG_SPAM, "RemoveProcessing (waiting on lock)");
            lock(DPLock)
            {
                Processing.Remove(entry);
            }
            report.Log(LogFlag.DEBUG_SPAM, "RemoveProcessing (released lock)");
        }
        public void ReQueue(DownloadEntryBase entry)
        {
            //TODO save optimal batch settings in config
            lock (DPLock)
            {
                Downloading.Remove(entry);
                Queued.Add(entry);
            }
            //---
            lock(fail_lock)
            {
                fails++;
                //BATCH_DELAY_MS += 200;
                BATCH_MIN_TIME += 200;
                if (BATCH_SIZE > 5 && fails > (BATCH_SIZE * 3/4))
                {
                    BATCH_MIN_TIME -= (100 * fails);
                    BATCH_SIZE--;
                    fails = 0;
                }
                
                report.Log(LogFlag.WARN,
                $"({fails}) Batch Delay {BATCH_DELAY_MS}\t" +
                $"Size {BATCH_SIZE}\t" +
                $"Min {BATCH_MIN_TIME}");
            }
            //---
        }
    }
}
