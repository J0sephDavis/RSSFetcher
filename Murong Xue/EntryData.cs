using Murong_Xue.DownloadHandling;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;
using System.Xml;

namespace Murong_Xue
{
    internal class EntryData(Uri RSSPath)
    {
        private readonly Uri path = RSSPath;
        private readonly Reporter report = Logger.RequestReporter("ENTDAT");
        //--------------------------------------------------------------------
        private const string RSS_Title = "title";
        private const string RSS_URL = "feed-url";
        private const string RSS_Expression = "expr";
        private const string RSS_History = "history";
        private const string RSS_Item = "item";
        private const string RSS_Date = "date";
        //--------------------------------------------------------------------
        public async Task Process()
        {
            if (GetEntries() == false)
            {
                report.Error("Failed to get entries");
                return;
            }
            //
            await DownloadFeeds();
            //save changes
            UpdateEntries();
        }
        public async Task DownloadFeeds()
        {
            report.Trace("Queueing Downloads");
            //OLD: Queueing Entries asynchronously saves 23ms (53ms sync, 32ms async)
            //NEW: Seems to be  faster to queue syncronously again,
            //previously we were creating objects every time, this time its just adding to a list
            foreach (var feed in Feeds)
            {
                var entry = new DownloadEntryFeed(feed);
                feed.Status |= FeedStatus.PROCESS;
            }
            await DownloadHandler.GetInstance().ProcessDownloads();

        }
        //! Reads the XML files and populated the FeedEntry list
        private bool GetEntries()
        {
            /* NOTE! Running the XMLReader in Async on our config file takes 23-26ms
             * Running Synchronously it takes 13-14ms                               */
            if (File.Exists(path.LocalPath) == false)
            {
                report.Error($"RSS Config File not found ({path})");
                return false;
            }
            FileStream xStream = System.IO.File.Open(path.LocalPath, FileMode.Open);
            XmlReaderSettings xSettings = new() { Async = false };

            using (XmlReader reader = XmlReader.Create(xStream, xSettings))
            {
                Feed feed = new();

                bool InTitle = false;
                bool InUrl = false;
                bool InExpr = false;
                bool InHistory = false;
                bool InDate = false;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case RSS_Title:
                                    InTitle = true;
                                    break;
                                case RSS_URL:
                                    InUrl = true;
                                    break;
                                case RSS_Expression:
                                    InExpr = true;
                                    break;
                                case RSS_History:
                                    InHistory = true;
                                    break;
                                case RSS_Date:
                                    InDate = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            if (InTitle)
                            {
                                feed.Title = reader.Value;
                            }
                            else if (InExpr)
                            {
                                feed.Expression = reader.Value;
                            }
                            else if (InHistory)
                            {
                                feed.History = reader.Value;
                            }
                            else if (InDate)
                            {
                                feed.Date = DateTime.Parse(reader.Value);
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case RSS_Title:
                                    InTitle = false;
                                    break;
                                case RSS_URL:
                                    InUrl = false;
                                    break;
                                case RSS_Expression:
                                    InExpr = false;
                                    break;
                                case RSS_History:
                                    InHistory = false;
                                    break;
                                case RSS_Date:
                                    InDate = false;
                                    break;
                                case RSS_Item:
                                    feed.ID = GetPrivateKey();
                                    feed.Status |= FeedStatus.FROM_FILE;
                                    AddFeed(feed);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.CDATA:
                            if (InUrl)
                            {
                                feed.URL = new(reader.Value);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            xStream.Dispose();
            return true;
        }
        public void AddFeed(Feed feed)
        {
            feed.Status |= FeedStatus.LINKED;
            Feeds.Add(new(feed));
            report.TraceVal($"FEED ADDED:\n{feed.ToLongString()}");
        }
        public bool RemoveFeed(Feed feed)
        {
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
        public void UpdateEntries()
        {
            Uri newFilePath = new(Path.ChangeExtension(path.LocalPath, null) + "_OLD.xml"); //insane that this is the easiest way without worrying about platform specific / & \
            Console.WriteLine($"newPath {newFilePath.LocalPath}");
            File.Move(path.LocalPath, newFilePath.LocalPath, overwrite: true);
            FileStream xStream = File.Open(path.LocalPath, FileMode.Create);

            //NOTICE: async saving takes ~4ms longer than just saving syncronously (6ms -> 1-2ms)
            XmlWriterSettings xSettings = new() { Async = false, Indent = true };
            DateTime _today = DateTime.Now;
            using (XmlWriter writer = XmlWriter.Create(xStream, xSettings))
            {
                //-------- ROOT
                writer.WriteStartElement(null, "root", null);
                TimeSpan SinceLast;
                //---- item
                foreach (var feed in Feeds)
                {
                    SinceLast = _today - feed.Date;
                    if (SinceLast.Days > 10)
                    {
                        report.Out($"{feed.Title} / {feed.URL} has not received an update in {SinceLast.Days} days");
                    }
                    //----
                    writer.WriteStartElement(RSS_Item);
                    // 1. Title
                    writer.WriteElementString(RSS_Title, feed.Title);
                    // 2. feed-url
                    writer.WriteStartElement(RSS_URL);
                    writer.WriteCData(feed.URL == null ? string.Empty : feed.URL.ToString());
                    writer.WriteEndElement();
                    // 3. expr
                    writer.WriteElementString(RSS_Expression, feed.Expression);
                    // 4. history
                    writer.WriteElementString(RSS_History, feed.History);
                    // 5. LastEntry date
                    writer.WriteElementString(RSS_Date, feed.Date.ToString());
                    //---- end item
                    writer.WriteEndElement();
                }
                //------- ROOT
                writer.WriteEndElement();
                writer.Flush();
            }
        }
    }
}
