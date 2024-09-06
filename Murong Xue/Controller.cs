using RSSFetcher.DownloadHandling;
using RSSFetcher.FeedData;
using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace RSSFetcher
{
    /// <summary>
    /// This class orchestrates the initiation and cooperation of the classes
    /// the bundle of states and methods to rule them all, per-se
    /// In no particular order, these are the operations the controller orchestrates
    /// >get Feeds from file
    /// >add feed from process (UI/&c)
    /// >process Feeds
    /// >edit Feeds from UI
    /// >save Feeds
    /// </summary>
    public class Controller
    {
        private DataFile rssData;
        private FeedManager feedManager;
        private Reporter report = Logger.RequestReporter("CONTRL");

        public Controller()
        {
            rssData = new(Config.GetInstance().GetRSSPath());
            feedManager = new();
            Init();
        }
        private async void Init()
        {
            feedManager.AddFeeds(rssData.ReadFeeds());
        }
        public void SubscribeFeedAddOrRemove(EventHandler method)
        {
            feedManager.FeedAddOrRemove += method;
        }
        //-----------Tasks------------------------------------------------------
        public List<Feed> GetFeeds()
        {
            return feedManager.GetFeeds();
        }
        public Feed? GetFeed(int ID)
        {
            return feedManager.GetFeed(ID);
        }
        public async Task DownloadFeeds()
        {
            report.Trace("DownloadFeeds()");
            foreach (var feed in GetFeeds())
            {
                var entry = new DownloadEntryFeed(feed);
            }
            await DownloadHandler.GetInstance().ProcessDownloads();
        }
        public void UpdateEntries()
        {
            rssData.WriteFeeds(GetFeeds());
        }
        //----------------------------------------------------------------------
        /// <summary>
        /// Add a feed to the model
        /// </summary>
        /// <param name="feed"></param>
        /// <returns>the feed to link in the model</returns>
        public bool CreateFeed(Feed feed)
        {
            report.Trace("Create Feed");
            if (feed.URL == null) return false;
            feedManager.AddFeed(feed);
            return true;
        }
        public bool DeleteFeed(Feed? feed)
        {
            if (feed == null)
            {
                report.Warn("Attempting to delete null feed");
                return false;
            }
            report.Trace($"Delete feed {feed}");
            bool IsRemoved = false;
            if ((feed.Status & FeedStatus.LINKED) != 0)
            {
                IsRemoved = feedManager.RemoveFeed(feed.ID);
                if (IsRemoved == false)
                    report.Warn("rss.RemoveFeed - feed not removed.");
                return IsRemoved;
            }
            else report.TraceVal($"RemoveFeed: feed not linked, status={feed.Status}");
            return IsRemoved;
        }
    }
}
