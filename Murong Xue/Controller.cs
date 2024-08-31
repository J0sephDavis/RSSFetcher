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
            //TODO when a feed is updated we must update the corresponding entry in Entries?
            //is the Feed ref linked still?
            await rss.DownloadFeeds();
        }
        public void UpdateEntries()
        {
            //! TODO add newly created entries to the RSS list
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

        public Feed? CreateNewFeedRecord()
        {
            Feed tmp = new()
            {
                ID = rss.GetPrivateKey()
            };
            Feeds.Add(tmp);
            return tmp;
        }
        public void AddFeedsToEntries(Feed feed)
        {
            if (feed.URL == null) return;
            //we are going to implicitly trust the caller that this feed does not already exist.
            rss.AddFeed(feed);
        }
        /// <summary>
        /// If the given feed is not represented by a FeedEntry in the model, it is sent to the model.
        /// Otherwise, nothing changes.
        /// </summary>
        /// <param name="feed"></param>
        public void  UpdateFeed(Feed feed)
        {
            report.Trace("Update Feed");
            if ((feed.Status & FeedStatus.LINKED) == 0)
                AddFeedsToEntries(feed);
        }
        public bool DeleteFeed(Feed feed)
        {
            report.Trace("Delete feed:");
            report.Trace(feed.ToString());
            //remove from rss
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
