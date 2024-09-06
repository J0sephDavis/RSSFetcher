using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;
using System.Xml;

namespace RSSFetcher.FeedData
{
    internal class DataFile(Uri RSSPath)
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
        //! Reads the XML files and populated the FeedEntry list
        public List<Feed> ReadFeeds()
        {
            List<Feed> Feeds = [];
            /* NOTE! Running the XMLReader in Async on our config file takes 23-26ms
             * Running Synchronously it takes 13-14ms                               */
            if (File.Exists(path.LocalPath) == false)
            {
                report.Error($"RSS Config File not found ({path})");
                return Feeds;//TODO throw an err
            }

            using FileStream xStream = File.Open(path.LocalPath, FileMode.Open);

            XmlReaderSettings xSettings = new() { Async = false };
            using XmlReader reader = XmlReader.Create(xStream, xSettings);

            Feed? feed = null;
            bool InTitle = false;
            bool InUrl = false;
            bool InExpr = false;
            bool InHistory = false;
            bool InDate = false;

            while (reader.Read())
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference. (Feed? feed)
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
                            case RSS_Item:
                                feed = new();
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
                                feed.Status |= FeedStatus.FROM_FILE;
                                Feeds.Add(feed);
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
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }

            return Feeds;
        }
        private void RenameOldFile()
        {
            report.Trace("RenameOldFile");
            if (File.Exists(path.LocalPath))
            {
                report.Debug("file does exist");
                Uri newFilePath = new(Path.ChangeExtension(path.LocalPath, null) + "_OLD.xml"); //insane that this is the easiest way without worrying about platform specific / & \
                report.TraceVal($"NewFilePath: {newFilePath}");
                File.Move(path.LocalPath, newFilePath.LocalPath, overwrite: true);
            }
            else report.Trace("file does not already exist");
        }
        public void WriteFeeds(List<Feed> feeds)
        {
            RenameOldFile();
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
                foreach (var feed in feeds)
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
