namespace Murong_Xue
{
    internal sealed class DownloadHandler
    {
        int BATCH_SIZE = 7;
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
            return s_DownloadHandler;
        }
        public async Task ProcessDownloads()
        {
            report.Notice("Processing downloads");
            List<Task> CurrentBatch = [];
            report.DebugVal($"Before processing, Queued[{Queued.Count}] Downloading[{Downloading.Count}]");
            
            int totalWaiting = Queued.Count + Downloading.Count + Processing.Count; // doesn't need a lock, no one is touching these arrays rn
            while (totalWaiting != 0) //these are volatile, might consider some type of sentinel/semaphore to handle this behavior
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
                        report.TraceVal($"\t\tQ[{Queued.Count}]  D[{Downloading.Count}]  P[{Processing.Count}]\tMIN TIME{BATCH_MIN_TIME}ms");
                        await Task.WhenAll(CurrentBatch);
                        CurrentBatch.Clear();
                    }
                }
                lock(DPLock)
                {
                    totalWaiting = Queued.Count + Downloading.Count + Processing.Count;
                    report.DebugVal($"Total waiting updated {totalWaiting} | Q[{Queued.Count}]  D[{Downloading.Count}]  P[{Processing.Count}]");
                }
                report.Trace("Delay 500ms");
                await Task.Delay(500); //nothing queued, may be wise to pass an autoresetevent to each of the download entries? or have some function that triggers it for them
            }
            report.DebugVal($"\t\tQ[{Queued.Count}]  D[{Downloading.Count}]  P[{Processing.Count}]");
            report.Notice("Download queue exhausted");
        }
        public void QueueDownload(DownloadEntryBase entry)
        {
            report.Trace("Add to Queue (waiting on lock)");
            lock (DPLock)
            {
                Queued.Add(entry);
            }
            report.Trace("Queued (releasing lock)");
        }
        private DownloadEntryBase PopSwapDownload()
        {
            report.Trace("PopSwap Q->D (waiting on lock)");
            DownloadEntryBase? entry = null;
            lock (DPLock)
            {
                entry = Queued.First();
                Queued.Remove(entry);
                Downloading.Add(entry);
            }
            report.Trace("PopSwapped (released lock)");
            return entry;
        }
        public void DownloadingToProcessing(DownloadEntryBase entry)
        {
            report.Trace("DownloadingToProcessing (Waiting on lock)");
            lock (DPLock)
            {
                Downloading.Remove(entry);
                Processing.Add(entry);
            }
            report.Trace("DownloadingToProcessing (released lock)");
        }
        public void RemoveProcessing(DownloadEntryBase entry)
        {
            report.Trace("RemoveProcessing (waiting on lock)");
            lock (DPLock)
            {
                Processing.Remove(entry);
            }
            report.Trace("RemoveProcessing (released lock)");
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
            lock (fail_lock)
            {
                fails++;
                BATCH_MIN_TIME += 200;
                if (BATCH_SIZE > 5 && fails > (BATCH_SIZE * 3/4))
                {
                    BATCH_MIN_TIME -= (100 * fails);
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
