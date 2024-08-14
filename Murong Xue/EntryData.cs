using Murong_Xue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Murong_Xue
{
    internal class EntryData
    {
        private Uri path;
        private List<FeedData> Feeds;
        private DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        private Reporter? report;// = new Reporter(LogFlag.DEFAULT, "EntryData");
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
            report.Log(LogFlag.DEBUG_SPAM, "Requesting entry data");
            if(GetEntries() == false)
            {
                report.Log(LogFlag.ERROR, "Failed to get entries");
                return;
            }
            //
            report.Log(LogFlag.DEBUG_SPAM, "Queueing Downloads");
            //Queueing feeds asynchronously saves 23ms (53ms sync, 32ms async)
            List<Task> taskList = [];
            foreach (FeedData feed in Feeds)
                taskList.Add(Task.Run(() => feed.QueueDownload()));
            await Task.WhenAll(taskList);
            //
            report.Log(LogFlag.DEBUG_SPAM, "Process Downloads");
            await downloadHandler.ProcessDownloads();
            //save changes
            report.Log(LogFlag.DEBUG_SPAM, "Save Changes");
            await UpdateEntries();
        }
        public List<FeedData> GetFeeds()
        {
            report.Log(LogFlag.DEBUG_SPAM, "Get Feeds");
            if (Feeds.Count == 0)
                GetEntries();
            return Feeds;
        }
        //! Reads the XML files and populated the FeedData list
        private bool GetEntries()
        {
            /* NOTE! Running the XMLReader in Async on our config file takes 23-26ms
             * Running Synchronously it takes 13-14ms                               */
            report.Log(LogFlag.DEBUG_SPAM, "Get Entries");
            if (File.Exists(path.LocalPath) == false)
            {
                report.Log(LogFlag.ERROR, $"RSS Config File not found ({path})");
                return false;
            }
            FileStream xStream = System.IO.File.Open(path.LocalPath, FileMode.Open);
            XmlReaderSettings xSettings = new();
            xSettings.Async = false;

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
                            switch(reader.Name)
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
        public async Task UpdateEntries()
        {
            report.Log(LogFlag.DEBUG, "Update Entries");
            Uri newFilePath = new Uri(Path.ChangeExtension(path.LocalPath, null) + "_OLD.xml"); //insane that this is the easiest way without worrying about platform specific / & \
            Console.WriteLine($"newPath {newFilePath.LocalPath}");
            File.Move(path.LocalPath, newFilePath.LocalPath, overwrite:true);
            FileStream xStream = File.Open(path.LocalPath, FileMode.Create);
            XmlWriterSettings xSettings = new();
            xSettings.Async = true;
            using (XmlWriter writer = XmlWriter.Create(xStream,xSettings))
            {
                //-------- ROOT
                await writer.WriteStartElementAsync(null,"root",null);
                //---- item
                foreach (FeedData feed in Feeds)
                {
                    await writer.WriteStartElementAsync(null, RSS_Item, null);
                    // 1. Title
                    await writer.WriteStartElementAsync(null, RSS_Title, null);
                    await writer.WriteStringAsync(feed.GetTitle());
                    await writer.WriteEndElementAsync();
                    // 2. feed-url
                    await writer.WriteStartElementAsync(null, RSS_URL, null);
                    await writer.WriteCDataAsync(feed.GetURL());
                    await writer.WriteEndElementAsync();
                    // 3. expr
                    await writer.WriteStartElementAsync(null, RSS_Expression, null);
                    await writer.WriteStringAsync(feed.GetExpr());
                    await writer.WriteEndElementAsync();
                    // 4. history
                    await writer.WriteStartElementAsync(null, RSS_History, null);
                    await writer.WriteStringAsync(feed.GetHistory());
                    await writer.WriteEndElementAsync();
                    //---- end item
                    await writer.WriteEndElementAsync();
                }
                //------- ROOT
                await writer.WriteEndElementAsync();
                await writer.FlushAsync();
            }

        }
    }
}
