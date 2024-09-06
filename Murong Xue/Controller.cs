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
        public async Task<List<Feed>> GetFeeds(bool LoadFromFile = false)
        {
            if (LoadFromFile)
                feedManager.AddFeeds(await rssData.ReadFeeds());
            // ---
            return feedManager.GetFeeds();
        }
        public Feed? GetFeed(int ID)
        {
            return feedManager.GetFeed(ID);
        }
        public async Task DownloadFeeds()
        {
            report.Trace("DownloadFeeds()");
            foreach (var feed in await GetFeeds())
            {
                var entry = new DownloadEntryFeed(feed);
            }
            await DownloadHandler.GetInstance().ProcessDownloads();
        }
        public async void UpdateEntries()
        {
            rssData.WriteFeeds(await GetFeeds());
        }
        //----------------------------------------------------------------------

        /// <summary>
        /// Get an empty feed record with an ID given by the Model + adds Feeds to view/controller list
        /// </summary>
        /// <returns></returns>
        public Feed? CreateNewFeedRecord()
        {
            Feed tmp = new()
            {
                ID = rss.GetPrivateKey()
            };
            Feeds.Add(tmp);
            return tmp;
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
            rss.AddFeed(feed);
            return true;
        }
        public bool DeleteFeed(Feed feed)
        {
            report.Trace("Delete feed:");
            report.Trace(feed.ToString());

            if (!rss.RemoveFeed(feed))
            {
                report.Warn("rss.RemoveFeed return false. feed not removed.");
                return false;
            }
            Feeds.Remove(feed);
            return true;
        }
    }
}
