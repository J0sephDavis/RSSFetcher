using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;

namespace Murong_Xue.DownloadHandling
{
    internal class DownloadHandler
    {
        //---------------------------------------------------------------------------
        private static DownloadHandler? s_DownloadHandler = null;
        private readonly static HttpClient client = new();
        private readonly static Reporter report = Logger.RequestReporter("DLHAND");
        private readonly static EventTicker events = EventTicker.GetInstance();
        //---------------------------------------------------------------------------
        public int BATCH_SIZE { get; protected set; } = 7;
        const int batch_delay_unit = 100;
        public int BATCH_ADD_DELAY { get; protected set; } = batch_delay_unit * 5/4;
        private readonly object fail_lock = new();
        private int fails = 0;
        //---c# version 12 does not have System.Threading.Lock, so we use Object()---
        private readonly object ListLocks = new();
        private readonly List<DownloadEntryBase> Downloads = [];
        //---------------------------------------------------------------------------
        public static DownloadHandler GetInstance()
        {
            s_DownloadHandler ??= new DownloadHandler();
            return s_DownloadHandler;
        }
        private int GetTotalHolds()
        {
            lock(ListLocks)
            {
                var WaitingList =
                from entry in Downloads
                where entry.Status > DownloadStatus.INITIALIZED
                    && entry.Status < DownloadStatus.PROCESSED
                select entry;
                return WaitingList.Count();
            }
        }
        public async Task ProcessDownloads()
        {
            //tmp
            int index = 0;
            report.Notice("Processing downloads");
            List<Task> CurrentBatch = [];
            //Reasons to NOT stop the while loop, we've got people doing work or wanting to do work.
            while (GetTotalHolds() != 0)
            {
                report.Debug("ALL ENTRY STATUS:");
                index = 0;
                for (int i = Downloads.Count-1; i >= 0; i--)
                {
                    if (CurrentBatch.Count >= BATCH_SIZE)
                        break;
                    //----

                    DownloadEntryBase entry = Downloads.ElementAt(i);

                    report.DebugVal($"{index++} - {entry.Status}");
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
            index = 0;
            report.Debug("ALL ENTRY STATUS:");
            foreach (var entry in Downloads)
            {
                report.DebugVal($"{index++} - {entry.Status}");
            }
            report.Notice("Download queue exhausted");
        }
        public void ReQueue(DownloadEntryBase entry)
        {
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

        public void AddDownload(DownloadEntryBase entry)
        {
            lock(ListLocks)
            {
                Downloads.Add(entry);
                entry.Status = DownloadStatus.WAITING;
            }
        }
    }
}
