using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace RSSFetcher.DownloadHandling
{
    internal class DownloadHandler(Uri _downloadPath)
    {
        private readonly HttpClient client = new();
        private readonly Reporter report = Logger.RequestReporter("DLHAND2");
        private readonly EventTicker events = EventTicker.GetInstance();
        // ---
        public Uri DownloadFolder = _downloadPath;
        // ---
        int BATCH_SIZE = 7;
        const int batch_delay_unit = 100;
        int BATCH_ADD_DELAY = batch_delay_unit * 5 / 4;
        readonly object fail_lock = new();
        int fails = 0;
        // ---
        readonly object ListLock = new();
        readonly List<DownloadEntryBase> Downloads = [];
        // ---
        public void SetPath(Uri path)
        {
            report.Trace($"SetPath: {path}");
            DownloadFolder = path;
        }
        // ---
        public void AddDownload(DownloadEntryBase entry)
        {
            lock (ListLock)
            {
                Downloads.Add(entry);
                entry.Status = DownloadStatus.WAITING;
            }
        }
        public void ReQueue(DownloadEntryBase entry)
        {
            entry.Status = DownloadStatus.WAITING; //download handler will pick this back up
            events.OnDownloadReQueued();
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
        // ---
        private int GetTotalHolds()
        {
            lock (ListLock)
            {
                var WaitingList =
                from entry in Downloads
                where entry.Status > DownloadStatus.INITIALIZED
                    && entry.Status < DownloadStatus.PROCESSED
                select entry;
                return WaitingList.Count();
            }
        }
        // ---
        public async Task ProcessDownloads()
        {
            report.Notice("Processing downloads");
            report.DebugVal("Downloads.Count =" + Downloads.Count);
            List<Task> CurrentBatch = [];
            //Reasons to NOT stop the while loop, we've got people doing work or wanting to do work.
            while (GetTotalHolds() != 0)
            {
                for (int i = Downloads.Count - 1; i >= 0; i--)
                {
                    if (CurrentBatch.Count >= BATCH_SIZE)
                        break;
                    //----

                    DownloadEntryBase entry = Downloads.ElementAt(i);
                    if (entry.Status == DownloadStatus.WAITING)
                    {
                        CurrentBatch.Add(Task.Run(() => entry.Request(client)));
                        report.Trace($"delay {BATCH_ADD_DELAY}");
                        await Task.Delay(BATCH_ADD_DELAY);
                    }
                }
                await Task.WhenAll(CurrentBatch);
                CurrentBatch.Clear();
                await Task.Delay(100); //TODO remove?
            }
            report.Notice("Download queue exhausted");
        }
    }
}
