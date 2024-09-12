using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;
using RSSFetcher.Summary;
using System.Xml;

namespace RSSFetcher.FeedData
{
    public class DataFile(Uri RSSPath) : ISummarizeable
    {
        private readonly Uri path = RSSPath;
        private readonly Reporter report = Logger.RequestReporter("ENTDAT");
        // --- Summary Data
        private int Loaded = 0;
        private int Written = 0;
        private readonly List<TimeSpan> ages = [];
        //--------------------------------------------------------------------
        private const string RSS_Title = "title";
        private const string RSS_URL = "feed-url";
        private const string RSS_Expression = "expr";
        private const string RSS_History = "history";
        private const string RSS_Item = "item";
        private const string RSS_Date = "date";

        string ISummarizeable.Name => "DataFile";

        //--------------------------------------------------------------------
        //! Reads the XML files and populated the FeedEntry list
        public List<Feed> ReadFeeds()
        {
            report.Trace($"Reading from {path}");
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
            DateTime DateNow = DateTime.Now;
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
                                Loaded++;
                                ages.Add(DateNow - feed.Date);
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
                Uri newFilePath = new(Path.ChangeExtension(path.LocalPath, null) + "_OLD.xml"); //insane that this is the easiest way without worrying about platform specific / & \
                report.TraceVal($"NewFilePath: {newFilePath}");
                File.Move(path.LocalPath, newFilePath.LocalPath, overwrite: true);
            }
            else report.Trace("file does not already exist");
        }
        public void WriteFeeds(List<Feed> feeds)
        {
            report.Trace($"Writing to {path}");
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
                ages.Clear();
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
                    // --- Summary info
                    Written++;
                    ages.Add(SinceLast);
                }
                //------- ROOT
                writer.WriteEndElement();
                writer.Flush();
            }
        }

        List<SummaryItem> ISummarizeable.GetSummary()
        {
            List<SummaryItem> items = [];
            // ---
            items.Add(new("Loaded",Loaded.ToString()));
            items.Add(new("Written", Written.ToString()));
            //---
            if (ages.Count > 0)
            {
                TimeSpan average = TimeSpan.Zero;
                TimeSpan max = TimeSpan.Zero;
                foreach (TimeSpan t in ages)
                {
                    if (t > max) max = t;
                    average += t;
                }
                average = average.Divide(ages.Count);
                items.Add(new("Oldest (days)", max.TotalDays.ToString()));
                items.Add(new("Average (days)", average.TotalDays.ToString()));
            }
            // ---
            return items;
        }
    }
}
