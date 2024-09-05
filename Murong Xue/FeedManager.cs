using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;

namespace Murong_Xue
{
    internal class FeedManager()
    {
        private readonly Reporter report = Logger.RequestReporter("FEED-M");
        //--------------------------------------------------------------------
        private int PrivateKey = 0;
        private readonly object PrivateKeyLock = new(); // a precaution
        public int GetPrivateKey()
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
                if (f.ID == -1)
                    f.ID = GetPrivateKey();
            }
            Feeds.AddRange(feeds);
        }
        public int AddFeed(Feed feed) //return ID or -1
        {
            throw new NotImplementedException();
            feed.Status |= FeedStatus.LINKED;
            Feeds.Add(new(feed));
            report.TraceVal($"FEED ADDED:\n{feed.ToLongString()}");
        }
        public bool RemoveFeed(int ID)
        {
            throw new NotImplementedException();
            feed.Status = FeedStatus.INIT;
            var feed_remove = from f in Feeds
                                where f.ID == feed.ID
                                select f;
            report.Debug($"GIVEN FEED: {feed.ID} {feed.Title}");
            if (feed_remove.Count() > 1)
            {
                report.Error($"RemoveFeed retrieved too many{feed_remove.Count()} feeds with query");
                foreach (var f in feed_remove)
                    report.Debug($"FOUND FEED: {f.ID} {f.Title}");
                return false;
            }
            var _feed = feed_remove.First();
            report.Debug($"Removing feed: {_feed.ID} {_feed.Title}");
            Feeds.Remove(_feed);
            return true;
        }
    }
}
