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
    public class DataFileTest
    {
        //TODO get assembly dir then path to test file
        Uri TestFilePath = new("D:\\VisualStudio Community Projects\\RSSFetcher\\UnitTesting\\rss-config-TEST-FILE.xml");

        [TestMethod()]
        public void ReadFeedsTest()
        {
            List<Feed> testData = [
                new() { Title="Feed A", URL = new("https://example.com/Feed-A-RSS"),
                Expression = ".*Feed A.*", History = "Feed A 10", Date = DateTime.Parse("9/6/2024 11:34:08 AM")},
                //---
                new() { Title="Feed B", URL = new("https://example.com/Feed-B-RSS"),
                Expression = ".*Feed B.*", History = "Feed B 10", Date = DateTime.Parse("9/7/2024 11:24:04 AM")},
                //---
                new() { Title="Feed C", URL = new("https://example.com/Feed-C-RSS"),
                Expression = ".*Feed C.*", History = "Feed C 10", Date = DateTime.Parse("9/4/2024 11:16:04 AM")},
                //---
                new() { Title="Feed D", URL = new("https://example.com/Feed-D-RSS"),
                Expression = ".*Feed D.*", History = "Feed D 10", Date = DateTime.Parse("9/9/2024 2:58:47 PM")},
            ];
            foreach (var f in testData) f.Status |= FeedStatus.FROM_FILE;

            //----
            DataFile testFile = new(TestFilePath);
            List<Feed> feeds = testFile.ReadFeeds();

            Assert.AreEqual(feeds.Count, testData.Count);
            for(int i = 0; i < testData.Count; i++)
                Assert.AreEqual(testData[i], feeds[i]);

        }
        /*[TestMethod()]
        public void WriteFeedsTest()
        {
            //TODO, how to compare output files
            Assert.Fail();
        }*/
    }
}
