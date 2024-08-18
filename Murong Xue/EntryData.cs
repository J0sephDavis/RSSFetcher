using System.Xml;

namespace Murong_Xue
{
    internal class EntryData
    {
        private readonly Uri path;
        private readonly List<FeedData> Feeds;
        private readonly DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        private readonly Reporter report;// = new Reporter(LogFlag.DEFAULT, "EntryData");
        private const string RSS_Title = "title";
        private const string RSS_URL = "feed-url";
        private const string RSS_Expression = "expr";
        private const string RSS_History = "history";
        private const string RSS_Item = "item";
        public EntryData(Uri RSSPath)
        {
            this.path = RSSPath;
            Feeds = [];
            report ??= Config.OneReporterPlease("EntryData");
        }
        public async Task Process()
        {
            if (GetEntries() == false)
            {
                report.Error("Failed to get entries");
                return;
            }
            //
            report.Trace("Queueing Downloads");
            //Queueing feeds asynchronously saves 23ms (53ms sync, 32ms async)
            List<Task> taskList = [];
            foreach (FeedData feed in Feeds)
                taskList.Add(Task.Run(() => feed.QueueDownload()));
            await Task.WhenAll(taskList);
            //
            await downloadHandler.ProcessDownloads();
            //save changes
            UpdateEntries();
        }
        public List<FeedData> GetFeeds()
        {
            report.Trace("GetFeeds()");
            if (Feeds.Count == 0)
                GetEntries();
            return Feeds;
        }
        //! Reads the XML files and populated the FeedData list
        private bool GetEntries()
        {
            /* NOTE! Running the XMLReader in Async on our config file takes 23-26ms
             * Running Synchronously it takes 13-14ms                               */
            report.Trace("GetEntries()");
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

                bool InTitle = false;
                bool InUrl = false;
                bool InExpr = false;
                bool InHistory = false;
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
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            if (InTitle)
                                _title = reader.Value;
                            else if (InUrl)
                                _url = reader.Value;
                            else if (InExpr)
                                _expr = reader.Value;
                            else if (InHistory)
                                _history = reader.Value;
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
                                case RSS_Item:
                                    Feeds.Add(new FeedData(
                                        title:      _title,
                                        url:        _url,
                                        expression: _expr,
                                        history:    _history)
                                    );
                                    Feeds.Last().Print();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.CDATA:
                            if (InUrl)
                                _url = reader.Value;
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
            report.Trace("UpdateEntries()");
            Uri newFilePath = new(Path.ChangeExtension(path.LocalPath, null) + "_OLD.xml"); //insane that this is the easiest way without worrying about platform specific / & \
            Console.WriteLine($"newPath {newFilePath.LocalPath}");
            File.Move(path.LocalPath, newFilePath.LocalPath, overwrite: true);
            FileStream xStream = File.Open(path.LocalPath, FileMode.Create);
            
            //NOTICE: async saving takes ~4ms longer than just saving syncronously (6ms -> 1-2ms)
            XmlWriterSettings xSettings = new() { Async = false };
            using (XmlWriter writer = XmlWriter.Create(xStream, xSettings))
            {
                //-------- ROOT
                writer.WriteStartElement(null, "root", null);
                //---- item
                foreach (FeedData feed in Feeds)
                {
                    writer.WriteStartElement(null, RSS_Item, null);
                    // 1. Title
                    writer.WriteStartElement(null, RSS_Title, null);
                    writer.WriteString(feed.GetTitle());
                    writer.WriteEndElement();
                    // 2. feed-url
                    writer.WriteStartElement(null, RSS_URL, null);
                    writer.WriteCData(feed.GetURL());
                    writer.WriteEndElement();
                    // 3. expr
                    writer.WriteStartElement(null, RSS_Expression, null);
                    writer.WriteString(feed.GetExpr());
                    writer.WriteEndElement();
                    // 4. history
                    writer.WriteStartElement(null, RSS_History, null);
                    writer.WriteString(feed.GetHistory());
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
