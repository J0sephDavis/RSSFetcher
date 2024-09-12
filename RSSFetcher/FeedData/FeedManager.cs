using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace RSSFetcher.FeedData
{
    public class FeedManager()
    {
        private readonly Reporter report = Logger.RequestReporter("FEED-M");
        //--------------------------------------------------------------------
        public event EventHandler FeedAddOrRemove = delegate { };
        //--------------------------------------------------------------------
        private int PrivateKey = 0;
        private readonly object PrivateKeyLock = new(); // a precaution
        private int GetPrivateKey()
        {
            lock (PrivateKeyLock)
            {
                return PrivateKey++;
            }
        }
        //--------------------------------------------------------------------
        private readonly List<Feed> Feeds = [];
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
        public void AddFeeds(List<Feed> feeds)
        {
            foreach (var f in feeds)
            {
                if (f.ID != -1) //check LINKED instead?
                    throw new ApplicationException("ID already exists");
                if (f.ID == -1)
                    f.ID = GetPrivateKey();
                f.Status |= FeedStatus.LINKED;
                report.TraceVal($"FEED ADDED:\n{f.ToLongString()}");
            }
            Feeds.AddRange(feeds);
            // --- events
            FeedAddOrRemove.Invoke(this, new EventArgs());
        }
        public void AddFeed(Feed feed) //return ID or -1
        {
            report.Trace($"AddFeed {feed}");
            if (feed.ID != -1)
            {
                throw new ApplicationException("ID already exists");
            }
            if (feed.ID == -1)
                feed.ID = GetPrivateKey();
            feed.Status |= FeedStatus.LINKED;
            Feeds.Add(feed);

            report.TraceVal($"FEED ADDED:\n{feed.ToLongString()}");
            // --- events
            FeedAddOrRemove.Invoke(this, new EventArgs());
        }
        /// <summary>
        /// Removes a feed from the managed list. Sets ID to -1
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool RemoveFeed(int ID)
        {
            var feed_remove =
                from f in Feeds
                where f.ID == ID
                select f;

            if (feed_remove.Count() > 1)
            {
                //TODO this should probably throw an exception
                report.Error($"RemoveFeed retrieved too many{feed_remove.Count()} feeds with query");
                foreach (var f in feed_remove)
                    report.Debug($"FOUND FEED: {f.ID} {f.Title}");
                return false;
            }
            var _feed = feed_remove.First();
            report.Debug($"Removing feed: {_feed.ID} {_feed.Title}");
            Feeds.Remove(_feed);
            //reset the feed left in the editor so that it can be resubmitted.
            _feed.ID = -1; //no ID if its not LINKED
            _feed.Status -= FeedStatus.LINKED; //if it weren't LINKED it wouldn't be here
            // --- events
            FeedAddOrRemove.Invoke(this, new());
            return true;
        }
        public Feed GetStub()
        {
            return new(GetPrivateKey());
        }
    }
}
