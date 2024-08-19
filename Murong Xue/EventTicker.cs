using System.Diagnostics;

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
        DateTime start = DateTime.Now;
        //---
        static EventTicker? s_EventTicker = null;
        private readonly Reporter report = Config.OneReporterPlease("EventTicker");

        private EventTicker()
        {  }
        public static EventTicker GetInstance()
        {
            s_EventTicker ??= new();
            return s_EventTicker;
        }
        public string GetSummary()
        {
            return "Event Summary:" +
                $"\n\tFeeds Checked: {feedsDownloaded}" +
                $"\n\tFiles Downloaded: {filesDownloaded}" +
                $"\n\tDownloads ReQueued: {downloadsRetried}" +
                $"\n\tDuration: {DateTime.Now - start}";
        }

        public void OnFeedDownloaded()
        {
            report.TraceVal("OnFeedDownloaded " + feedsDownloaded);
            feedsDownloaded++;
        }
        public void OnFileDownloaded()
        {
            report.TraceVal("OnFileDownloaded " + filesDownloaded);
            filesDownloaded++;
        }
        public void OnDownloadReQueued()
        {
            report.TraceVal("OnDownloadeReQueued " + downloadsRetried);
            downloadsRetried++;
        }
#if false
        List<TimeSpan> sw_record = [];
        public void HandleStopWatch(TimeSpan elapsed_t)
        {
            lock (sw_record)
            {
                sw_record.Add(elapsed_t);
            }
            /*Used in GetSummary():
            foreach (var t in sw_record)
            {
                report.Out(t.ToString());
            }
            TimeSpan avg_time =
                sw_record.Aggregate((accumulated, current) => accumulated + current)
                / sw_record.Count;
            report.Out("AVERAGE STOPWATCH TIME: " + avg_time);
            */
            //events.HandleStopWatch(stopwatch.Elapsed);
        }
#endif
    }
}
