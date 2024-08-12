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
        //"fails" will be used for  batch size & delay adjustments
        private readonly object fail_lock = new object();
        private uint fails = 0;
        //---
        private static DownloadHandler? s_DownloadHandler = null;
        private static HttpClient client = new();
        //c# version 12 does not have System.Threading.Lock, so we use Object()
        private readonly object DPLock = new(); //for when swapping between the two lists.
        //list of files to be downloaded
        private List<DownloadEntryBase> Downloads = [];
        //Not in download, but also not done.
        private List<DownloadEntryBase> Processing = [];
        private static Reporter report = new Reporter(LogFlag.DEFAULT, "Download Handler");
        
        private DownloadHandler()
        { }
        public static DownloadHandler GetInstance()
        {
            report.Log(LogFlag.DEBUG_SPAM, "GetInstance");
            if (s_DownloadHandler == null)
                s_DownloadHandler = new DownloadHandler();
            return s_DownloadHandler;
        }
        public async Task ProcessDownloads()
        {
            report.Log(LogFlag.DEBUG, "Processing Downloads");
            List<Task> CurrentBatch = [];
            List<DownloadEntryBase> tmpList = [];
            report.Log(LogFlag.DEBUG, $"Before processing, Downloads[{Downloads.Count}] Processing[{Processing.Count}]");
            while (Downloads.Count != 0 || Processing.Count != 0)
            {
                while (Downloads.Count != 0)
                {
                    DownloadEntryBase entry = PopSwapDownload();
                    //Add the entry to the task list
                    CurrentBatch.Add(entry.Request(client));
                    //When we've filled our budget or used em all
                    if (CurrentBatch.Count >= BATCH_SIZE || Downloads.Count == 0)
                    {
                        report.Log(LogFlag.DEBUG, $"BATCH[{CurrentBatch.Count}]! Downloads[{Downloads.Count}] Processing[{Processing.Count}]");
                        await Task.WhenAll(CurrentBatch);
                        report.Log(LogFlag.DEBUG_SPAM, "Cleared currentBatch");
                        CurrentBatch.Clear();
                        report.Log(LogFlag.DEBUG_SPAM, $"Wait {BATCH_DELAY_MS}ms");
                        await Task.Delay(BATCH_DELAY_MS);
                    }
                }
            }
            report.Log(LogFlag.NOTEWORTHY, "Downloads Processed!");
        }
        public void AddDownload(DownloadEntryBase entry)
        {
            report.Log(LogFlag.DEBUG_SPAM, "Add Download (waiting on lock)");
            lock(DPLock)
            {
                Downloads.Add(entry);
                report.Log(LogFlag.DEBUG_SPAM, "Download Added (releasing lock)");
            }
        }
        private DownloadEntryBase PopSwapDownload()
        {
            report.Log(LogFlag.DEBUG_SPAM, "PopSwap (waiting on lock)");
            DownloadEntryBase? entry = null;
            lock (DPLock)
            {
                entry = Downloads.First();
                Downloads.Remove(entry);
                Processing.Add(entry);
                report.Log(LogFlag.DEBUG_SPAM, "PopSwapped (releasing lock)");
            }
            return entry;
        }
        public void RemoveProcessing(DownloadEntryBase entry)
        {
            report.Log(LogFlag.DEBUG_SPAM, "Remove Processing (Waiting on lock)");
            lock (DPLock)
            {
                Processing.Remove(entry);
                report.Log(LogFlag.DEBUG_SPAM, "Removed from processing (releasing lock)");
            }
        }
        public void ReQueue(DownloadEntryBase entry)
        {
            //TODO save optimal batch settings in config
            lock (DPLock)
            {
                Processing.Remove(entry);
                Downloads.Add(entry);
            }
            //---
            lock(fail_lock)
            {
                fails++;
                BATCH_DELAY_MS += 200;
                if (BATCH_SIZE > 5 && fails > BATCH_SIZE / 2)
                {
                    BATCH_SIZE--;
                    fails = 0;
                }
                
                report.Log(LogFlag.WARN,
                $"({fails}) Batch Delay {BATCH_DELAY_MS}\t" +
                $"Size {BATCH_SIZE}");
            }
            //---
        }
    }
}
