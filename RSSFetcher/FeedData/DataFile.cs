using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;
using RSSFetcher.Summary;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using System.Xml;
using System.Xml.Serialization;

namespace RSSFetcher.FeedData
{
    partial class DataFile : ISummarizeable
    {
        // --- Summary Data
        private int Loaded = 0;
        private int Written = 0;
        private readonly List<TimeSpan> ages = [];
        // ---
        string ISummarizeable.Name => "DataFile";
        List<SummaryItem> ISummarizeable.GetSummary()
        {
            List<SummaryItem> items = [];
            // ---
            items.Add(new("Loaded", Loaded.ToString()));
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
    
    //[SerializableAttribute()]
    [XmlTypeAttribute("item",AnonymousType = true)]
    [XmlRootAttribute("root",IsNullable = false)]
    public partial class xmlRoot
    {
        [XmlElementAttribute("item")]
        public xmlFeed[] item { get; set; }
    }

    //[SerializableAttribute()]
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class xmlFeed
    {
        public string title { get; set; }

        [XmlElementAttribute("feed-url")]
        public string feedurl { get; set; }
        public string expr { get; set; }
        public string history { get; set; }
        public string date { get; set; }
        public Feed ToFeed()
        {
            return new()
            {
                Title = title,
                URL = new(feedurl),
                Expression = expr,
                History = history,
                Date = DateTime.Parse(date),
                Status = FeedStatus.INIT | FeedStatus.FROM_FILE,
            };
        }
    }
    public partial class DataFile(Uri RSSPath)
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
            List<Feed> feeds = [];
            // ---
            report.Trace($"Reading from {path}");
            if (File.Exists(path.LocalPath) == false)
            {
                report.Error($"RSS Config File not found ({path})");
                return feeds;//TODO throw an err
            }

            using FileStream xStream = File.Open(path.LocalPath, FileMode.Open);

            XmlReaderSettings xSettings = new() { Async = false };
            using XmlReader reader = XmlReader.Create(xStream, xSettings);
            // ---
            XmlSerializer serializer = new XmlSerializer(typeof(xmlRoot));
            xmlRoot? document = serializer.Deserialize(reader) as xmlRoot;
            //
            if (document == null)
                throw new ApplicationException("Failed to deserialize xml");

            foreach (var item in document.item)
                feeds.Add(item.ToFeed());
            // ---
            return feeds;
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
                writer.WriteStartElement(null, "xmlRoot", null);
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
                    if (feed.URL == null) report.Error($"FEED: {feed.Title} is MISSING a URL. (this shouldn't be possible)");
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
    }
}
