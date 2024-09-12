using Microsoft.VisualStudio.TestTools.UnitTesting;
using RSSFetcher.FeedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSFetcher.FeedData.Tests
{
    [TestClass]
    public class FeedManagerTest
    {
        //TODO EventHandler FeedAddOrRemove is currently untested
        [TestMethod]
        public void GetStub()
        {
            FeedManager manager = new();

            Feed feed = manager.GetStub();
            // ---
            Assert.AreEqual(feed.ID, 0); //first feed is always 0
            Assert.AreEqual(feed.Title, string.Empty);
            Assert.AreEqual(feed.URL, null);
            Assert.AreEqual(feed.Expression, string.Empty);
            Assert.AreEqual(feed.Date, DateTime.UnixEpoch);
            Assert.AreEqual(feed.History, string.Empty);
            Assert.AreEqual(feed.Status, FeedStatus.STUB);
        }
        [TestMethod]
        public void AddFeeds()
        {
            FeedManager manager = new();
            List<Feed> feeds =
            [
                new() { Title = "feed A"},
                new() { Title = "feed B"},
                new() { Title = "feed C"},
                new() { Title = "feed D"},
                new() { Title = "feed E"},
                new() { Title = "feed F"},
                new() { Title = "feed G"},
                new() { Title = "feed H"},
            ];
            foreach (var feed in feeds)
                Assert.AreEqual(feed.ID, -1);
            manager.AddFeeds(feeds);

            List<Feed> feedsFromManager = manager.GetFeeds();
            for (int i = 0; i < feeds.Count; i++)
            {
                Assert.AreNotEqual(feeds.ElementAt(i).ID, -1); //all feeds should be assigned an ID after being added
                Assert.AreEqual(feeds.ElementAt(i), feedsFromManager.ElementAt(i)); //records should be the same object, no copies made.
            }
        }
        [TestMethod]
        public void AddFeeds_AlreadyInitializedException()
        {
            FeedManager manager = new();
            List<Feed> feeds =
            [
                new() { Title = "feed A"},
                new() { Title = "feed B"},
                new() { Title = "feed C"},
                new() { Title = "feed D"},
                new() { Title = "feed E"},
                new() { Title = "feed F"},
                new() { Title = "feed G"},
                new() { Title = "feed H"},
            ];
            manager.AddFeeds(feeds);

            // Adding feeds which have already been added throws AppException (probably needlessly, could just silently discard)
            Assert.ThrowsException<ApplicationException>(
                () => manager.AddFeed(feeds.ElementAt(0))
            );
            Assert.ThrowsException<ApplicationException>(
                () => manager.AddFeeds(feeds.GetRange(0, 3))
            );
        }
        [TestMethod]
        public void RemoveFeed()
        {
            FeedManager manager = new();
            List<Feed> feeds =
            [
                new() { Title = "feed A"},
                new() { Title = "feed B"},
                new() { Title = "feed C"},
                new() { Title = "feed D"},
                new() { Title = "feed E"},
                new() { Title = "feed F"},
                new() { Title = "feed G"},
                new() { Title = "feed H"},
            ];
            manager.AddFeeds(feeds);
            // ---
            foreach (var feed in feeds)
            {
                Assert.AreEqual(manager.RemoveFeed(feed.ID), true);
                Assert.AreEqual(feed.ID, -1);
                Assert.AreEqual((int)(feed.Status & FeedStatus.LINKED), 0);
            }
        }
    }
}
