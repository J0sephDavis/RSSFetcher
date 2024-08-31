using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;

namespace Murong_Xue
{
    public class Controller
    {
        private EntryData rss;
        private List<Feed> Feeds = [];
        private Reporter report = Logger.RequestReporter("CONTRL");

        public Controller()
        {
            rss = new EntryData(Config.GetInstance().GetRSSPath());
            List<FeedEntry> Entries = rss.GetFeeds();
            foreach (FeedEntry entry in Entries)
                Feeds.Add((entry.GetFeed())); //creates a copy of each record,
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
