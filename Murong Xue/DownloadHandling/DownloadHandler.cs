using Murong_Xue.Logging;

namespace Murong_Xue.DownloadHandling
{
    internal sealed class DownloadHandler
    {
        //---------------------------------------------------------------------------
        private static DownloadHandler? s_DownloadHandler = null;
        private readonly static HttpClient client = new();
        private readonly static Reporter report = Config.OneReporterPlease("DLHAND");
        private readonly static EventTicker events = EventTicker.GetInstance();
        //---------------------------------------------------------------------------
        int BATCH_SIZE = 7;
        int BATCH_MIN_TIME = 1000;
        private readonly object fail_lock = new();
        private int fails = 0;
        //---c# version 12 does not have System.Threading.Lock, so we use Object()---
        private readonly object ListLocks = new();
        private readonly List<DownloadEntryBase> Queued = [];
        private readonly List<DownloadEntryBase> Processing = [];
        //---------------------------------------------------------------------------
        public static DownloadHandler GetInstance()
        {
            s_DownloadHandler ??= new DownloadHandler();
            return s_DownloadHandler;
        }
        public async Task ProcessDownloads()
        {
            report.Notice("Processing downloads");
            List<Task> CurrentBatch = [];

            int totalWaiting = Queued.Count + Processing.Count;
            report.DebugVal($"{totalWaiting} feeds queued for download");
            while (totalWaiting != 0)
            {
                while (Queued.Count != 0) //volatile but we do not care that much (it will be run again when totalWaiting is calculated)
                {
                    DownloadEntryBase entry = PopSwapDownload();
                    //Add the entry to the task list
                    CurrentBatch.Add(Task.Run(() => entry.Request(client)));
                    //When we've filled our budget or used em all
                    if (CurrentBatch.Count >= BATCH_SIZE || Queued.Count == 0)
                    {
                        CurrentBatch.Add(Task.Delay(BATCH_MIN_TIME));
                        report.TraceVal($"\t\tQ[{Queued.Count}]  P[{Processing.Count}]\tMIN TIME{BATCH_MIN_TIME}ms");
                        await Task.WhenAll(CurrentBatch);
                        CurrentBatch.Clear();
                    }
                }
                lock (ListLocks)
                {
                    totalWaiting = Queued.Count + Processing.Count;
                }
                await Task.Delay(100);
            }
            report.DebugVal($"\t\tQ[{Queued.Count}] P[{Processing.Count}]");
            report.Notice("Download queue exhausted");
        }
        public void QueueDownload(DownloadEntryBase entry)
        {
            lock (ListLocks)
            {
                Queued.Add(entry);
            }
        }
        private DownloadEntryBase PopSwapDownload()
        {
            DownloadEntryBase? entry = null;
            lock (ListLocks)
            {
                entry = Queued.First();
                Queued.Remove(entry);
                Processing.Add(entry);
            }
            return entry;
        }
        public void RemoveProcessing(DownloadEntryBase entry)
        {
            lock (ListLocks)
            {
                Processing.Remove(entry);
            }
        }
        public void ReQueue(DownloadEntryBase entry)
        {
            events.OnDownloadReQueued();
            lock (ListLocks)
            {
                Processing.Remove(entry);
                Queued.Add(entry);
            }
            //---
            lock (fail_lock)
            {
                fails++;
                BATCH_MIN_TIME += 200;
                if (BATCH_SIZE > 5 && fails > BATCH_SIZE * 3 / 4)
                {
                    BATCH_MIN_TIME -= 100 * fails;
                    BATCH_SIZE--;
                    fails = 0;
                }

                report.WarnSpam(
                    $"({fails}) Batch " +
                    $"Size {BATCH_SIZE}\t" +
                    $"Min {BATCH_MIN_TIME}"
                );
            }
        }
    }
}
