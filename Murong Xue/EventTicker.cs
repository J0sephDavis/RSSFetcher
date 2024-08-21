using System.Text;
using Murong_Xue.Logging;

namespace Murong_Xue
{
    /// <summary>
    /// To keep counts for each event that has happened & print at the end
    /// </summary>
    internal class EventTicker
    {
        private class StatLock
        {
            private int stat;
            public object padlock = new();
            public static StatLock operator --(StatLock a)
            {
                lock (a.padlock)
                {
                    a.stat--;
                }
                return a;
            }
            public static StatLock operator ++(StatLock a)
            {
                lock (a.padlock)
                {
                    a.stat++;
                }
                return a;
            }
            public override string ToString()
            {
                return stat.ToString();
            }
        }
        private StatLock feedsDownloaded = new();
        private StatLock filesDownloaded = new();
        private StatLock downloadsRetried = new();
        private readonly DateTime start = DateTime.Now;
        //---
        static EventTicker? s_EventTicker = null;
        //private readonly Reporter report = Config.OneReporterPlease("EVENTT");

        private EventTicker()
        { }
        public static EventTicker GetInstance()
        {
            s_EventTicker ??= new();
            return s_EventTicker;
        }
        public string GetSummary()
        {
            StringBuilder summary = new("Summary|");
            summary.Append($" Feeds {feedsDownloaded}");
            summary.Append($" Downloads: {filesDownloaded}");
            summary.Append($" fails: {downloadsRetried}");
            summary.Append($" Duration: {DateTime.Now - start}");
#if STOPWATCH
            foreach (var t in sw_record)
            {
                report.Out(t.ToString());
            }
            TimeSpan avg_time =
                sw_record.Aggregate((accumulated, current) => accumulated + current)
                / sw_record.Count;
            report.Out("AVERAGE STOPWATCH TIME: " + avg_time);
            summary.Append($" Avg SW Time {avg_time} of {sw_record.Count}");
#endif
            return summary.ToString();
        }

        public void OnFeedDownloaded()
        {
            feedsDownloaded++;
        }
        public void OnFileDownloaded()
        {
            filesDownloaded++;
        }
        public void OnDownloadReQueued()
        {
            downloadsRetried++;
        }
#if STOPWATCH
        List<TimeSpan> sw_record = [];
        public void HandleStopWatch(TimeSpan elapsed_t)
        {
            lock (sw_record)
            {
                sw_record.Add(elapsed_t);
            }
        }
#endif
    }
}
