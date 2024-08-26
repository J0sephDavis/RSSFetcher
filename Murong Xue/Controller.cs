namespace Murong_Xue
{
    public class Controller
    {
        private EntryData rss;
        private List<Feed> Feeds = [];

        public Controller()
        {
            rss = new EntryData(Config.GetInstance().GetRSSPath());
            List<FeedEntry> Entries = rss.GetFeeds();
            foreach (FeedEntry entry in Entries)
                Feeds.Add(new(entry.GetFeed())); //creates a copy of each record,
        }
        //----------------------------------------
        public async void DownloadFeeds()
        {
            //TODO when a feed is updated we must update the corresponding entry in Entries?
            //is the Feed ref linked still?
            await rss.DownloadFeeds();
        }
        public void UpdateEntries()
        {
            rss.UpdateEntries();
        }
        //----------------------------------------
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
        public int UpdateFeed(Feed feed)
        {
            Feed? currentFeed = GetFeed(feed.ID);
            if (currentFeed == null) return -1;
            currentFeed = feed; //this should update it?
            return feed.ID;
        }
        public int CreateFeed(Feed feed)
        {
            if (GetFeed(feed.ID) != null)
                return -1;
            Feeds.Add(feed);
            return feed.ID;
        }
        public int DeleteFeed(Feed feed)
        {
            Feeds.Remove(feed);
            return feed.ID;
        }
    }
}
