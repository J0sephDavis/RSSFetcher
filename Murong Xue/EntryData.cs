using System.Xml;
using Murong_Xue.DownloadHandling;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;

namespace Murong_Xue
{
    internal class EntryData(Uri RSSPath)
    {
        private readonly Uri path = RSSPath;
        private readonly List<FeedEntry> Feeds = [];
        private readonly Reporter report = Logger.RequestReporter("ENTDAT");
        private const string RSS_Title = "title";
        private const string RSS_URL = "feed-url";
        private const string RSS_Expression = "expr";
        private const string RSS_History = "history";
        private const string RSS_Item = "item";
        private const string RSS_Date = "date";

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
        public  async Task DownloadFeeds()
        {
            report.Trace("Queueing Downloads");
            //OLD: Queueing Entries asynchronously saves 23ms (53ms sync, 32ms async)
            //NEW: Seems to be  faster to queue syncronously again,
            //previously we were creating objects every time, this time its just adding to a list
            foreach (FeedEntry feed in Feeds)
                feed.Queue();
            await DownloadHandler.GetInstance().ProcessDownloads();

        }
        public List<FeedEntry> GetFeeds()
        {
            if (Feeds.Count == 0)
                GetEntries();
            return Feeds;
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
                string _title = string.Empty;
                string _url = string.Empty;
                string _expr = string.Empty;
                string _history = string.Empty;
                string _date = string.Empty;

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
                                _title = reader.Value;
                            }
                            else if (InExpr)
                            {
                                _expr = reader.Value;
                            }
                            else if (InHistory)
                            {
                                _history = reader.Value;
                            }
                            else if (InDate)
                            {
                                _date = reader.Value;
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
                                    if (_date == string.Empty)
                                        _date = DateTime.UnixEpoch.ToString();
                                    Feeds.Add(new FeedEntry(
                                        Title:      _title,
                                        URL:        new Uri(_url), //TODO make _url a URI when we get it, not here.
                                        Expression: _expr,
                                        History:    _history,
                                        Date:       _date)
                                    );
                                    Feeds.Last().Print();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.CDATA:
                            if (InUrl)
                            {
                                _url = reader.Value;
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
        public void UpdateEntries()
        {
            Uri newFilePath = new(Path.ChangeExtension(path.LocalPath, null) + "_OLD.xml"); //insane that this is the easiest way without worrying about platform specific / & \
            Console.WriteLine($"newPath {newFilePath.LocalPath}");
            File.Move(path.LocalPath, newFilePath.LocalPath, overwrite: true);
            FileStream xStream = File.Open(path.LocalPath, FileMode.Create);
            
            //NOTICE: async saving takes ~4ms longer than just saving syncronously (6ms -> 1-2ms)
            XmlWriterSettings xSettings = new() { Async = false };
            DateTime _today = DateTime.Now;
            using (XmlWriter writer = XmlWriter.Create(xStream, xSettings))
            {
                //-------- ROOT
                writer.WriteStartElement(null, "root", null);
                TimeSpan SinceLast;
                //---- item
                foreach (FeedEntry feed in Feeds)
            {
                    SinceLast = _today - feed.Date;
                    if (SinceLast.Days > 10)
                    {
                        report.Out($"{feed.Title} / {feed.URL} has not received an update in {SinceLast.Days} days");
                    }
                    //----
                    writer.WriteStartElement(RSS_Item);
                    // 1. Title
                    writer.WriteStartElement(RSS_Title);
                    writer.WriteString(feed.Title);
                    writer.WriteEndElement();
                    // 2. feed-url
                    writer.WriteStartElement(RSS_URL);
                    writer.WriteCData(feed.URL.ToString());
                    writer.WriteEndElement();
                    // 3. expr
                    writer.WriteStartElement(RSS_Expression);
                    writer.WriteString(feed.Expression);
                    writer.WriteEndElement();
                    // 4. history
                    writer.WriteStartElement(RSS_History);
                    writer.WriteString(feed.History);
                    writer.WriteEndElement();
                    // 5. LastEntry date
                    writer.WriteStartElement(RSS_Date);
                    writer.WriteString(feed.Date.ToString());
                    writer.WriteEndElement();
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
