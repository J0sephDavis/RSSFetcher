using Murong_Xue.Logging;

namespace Murong_Xue.DownloadHandling
{
    internal class DownloadHandler
    {
        //---------------------------------------------------------------------------
        private static DownloadHandler? s_DownloadHandler = null;
        private readonly static HttpClient client = new();
        private readonly static Reporter report = Config.OneReporterPlease("DLHAND");
        private readonly static EventTicker events = EventTicker.GetInstance();
        //---------------------------------------------------------------------------
        public int BATCH_SIZE { get; protected set; } = 7;
        const int batch_delay_unit = 100;
        public int BATCH_ADD_DELAY { get; protected set; } = batch_delay_unit * 5/4;
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
                while (Queued.Count != 0)
                {
                    report.Trace($"delay {BATCH_ADD_DELAY}");
                    await Task.Delay(BATCH_ADD_DELAY);
                    DownloadEntryBase entry = PopSwapDownload();
                    CurrentBatch.Add(Task.Run(() => entry.Request(client)));
                    //When we've filled our budget or used em all
                    if (CurrentBatch.Count >= BATCH_SIZE || Queued.Count == 0)
                    {
                        report.TraceVal($"\t\tQ[{Queued.Count}]  P[{Processing.Count}]");
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
                if (fails > BATCH_SIZE * 9 / 16)
                {
                    BATCH_ADD_DELAY -= batch_delay_unit / 8;
                    fails = 0;
                }
                else
                {
                    BATCH_ADD_DELAY += batch_delay_unit / 4;
                }

                report.WarnSpam(
                    $"({fails}) Batch " +
                    $"Size {BATCH_SIZE}\t" +
                    $"delay {BATCH_ADD_DELAY}"
                );
            }
        }
    }
}
