using Murong_Xue.DownloadHandling;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;
using System.ComponentModel;

namespace Murong_Xue
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
            init();
        }
        private async void init()
        {
            feedManager.AddFeeds(await rssData.ReadFeeds());
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
        public Feed CreateNewFeedRecord()
        {
            report.Trace("CreateNewFeedRecord");
            return new();
        }
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
        public bool DeleteFeed(Feed feed) => DeleteFeed(feed.ID);
        public bool DeleteFeed(int ID)
        {
            report.Trace($"Delete feed {ID}");
#if DEBUG
            Feed? feed = GetFeed(ID);
            if (feed != null)
                report.Trace(feed.ToString());
#endif
            bool IsRemoved = feedManager.RemoveFeed(ID);
            if (IsRemoved == false)
            {
                report.Warn("rss.RemoveFeed return false. feed not removed.");
            }
            
            return IsRemoved;
        }
    }
}
