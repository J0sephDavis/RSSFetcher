using RSSFetcher.FeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSFetcher.FeedData.Tests
{
    [TestClass]
    public class FeedTest
    {
        [TestMethod]
        public void DefaultConstructor()
        {
            Feed feed = new();
            Assert.AreEqual(feed.ID, -1);
            Assert.AreEqual(feed.Title, string.Empty);
            Assert.AreEqual(feed.URL, null);
            Assert.AreEqual(feed.Expression, string.Empty);
            Assert.AreEqual(feed.Date, DateTime.UnixEpoch);
            Assert.AreEqual(feed.History, string.Empty);
            Assert.AreEqual(feed.Status, FeedStatus.INIT);
        }
        [TestMethod]
        public void PrivateKeyConstructor()
        {
            int ID = 9999;
            Feed feed = new(9999);
            Assert.AreEqual(feed.ID, 9999);
            Assert.AreEqual(feed.Title, string.Empty);
            Assert.AreEqual(feed.URL, null);
            Assert.AreEqual(feed.Expression, string.Empty);
            Assert.AreEqual(feed.Date, DateTime.UnixEpoch);
            Assert.AreEqual(feed.History, string.Empty);
            Assert.AreEqual(feed.Status, FeedStatus.STUB);
        }
        [TestMethod]
        public void CopyConstructor()
        {
            Feed feed_A = new(10);
            feed_A.Title = "title_test";
            feed_A.URL = new("https://example.com/");
            feed_A.Expression = ".*";
            feed_A.Date = DateTime.Now;
            feed_A.Status = FeedStatus.INIT | FeedStatus.LINKED | FeedStatus.MODIFIED | FeedStatus.FROM_FILE | FeedStatus.DLHANDLE | FeedStatus.STUB;
            Feed feed_B = new(feed_A);

            Assert.AreEqual(feed_A, feed_B);
            Assert.AreEqual(feed_A.ID,          feed_B.ID);
            Assert.AreEqual(feed_A.Title,       feed_B.Title);
            Assert.AreEqual(feed_A.URL,         feed_B.URL);
            Assert.AreEqual(feed_A.Expression,  feed_B.Expression);
            Assert.AreEqual(feed_A.Date,        feed_B.Date);
            Assert.AreEqual(feed_A.History,     feed_A.History);
            Assert.AreEqual(feed_A.Status,      feed_B.Status);
        }
    }
}
