using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;

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
        private EntryData rss;
        private List<Feed> Feeds = [];
        private Reporter report = Logger.RequestReporter("CONTRL");

        public Controller()
        {
            rss = new EntryData(Config.GetInstance().GetRSSPath());
            Feeds = new(rss.GetFeeds()); //copy the list
        }
        //-----------Tasks------------------------------------------------------
        public async void DownloadFeeds()
        {
            await rss.DownloadFeeds();
        }
        public void UpdateEntries()
        {
            rss.UpdateEntries();
        }
        //----------------------------------------------------------------------
        public List<Feed> GetFeeds()
        {
            return Feeds;
        }
        public Feed? GetFeed(int ID)
        {
            foreach (var feed in Feeds)
                if (feed.ID == ID)
                    return feed;
            return null;
        }

        /// <summary>
        /// Get an empty feed record with an ID given by the Model + adds feeds to view/controller list
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
