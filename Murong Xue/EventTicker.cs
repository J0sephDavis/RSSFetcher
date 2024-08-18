namespace Murong_Xue
{
    /// <summary>
    /// To keep counts for each event that has happened & print at the end
    /// </summary>
    internal class EventTicker
    {
        private class statLock
        {
            private int stat;
            public object padlock = new();
            public static statLock operator --(statLock a)
            {
                lock (a.padlock)
                {
                    a.stat--;
                }
                return a;
            }
            public static statLock operator ++(statLock a)
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
        private statLock feedsDownloaded = new();
        private statLock filesDownloaded = new();
        private statLock downloadsRetried = new();
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
    }
}
