using MurongXue;
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
        private Config config = Config.GetInstance();
        private const string RSS_Title = "title";
        private const string RSS_URL = "feed-url";
        private const string RSS_Expression = "expr";
        private const string RSS_History = "history";
        private const string RSS_Item = "item";
        public EntryData(Uri RSSPath)
        {
            this.path = RSSPath;
            Feeds = new List<FeedData>();
        }
        public async Task Process()
        {
            if(GetEntries() == false)
            {
                Console.WriteLine("failed to get entries");
                return;
            }
            //
            foreach(FeedData feed in Feeds)
                feed.QueueDownload();
            //
            await downloadHandler.ProcessDownloads();
            //save changes
            await UpdateEntries();
        }
        //! Reads the XML files and populated the FeedData list
        private bool GetEntries()
        {
            if (File.Exists(path.LocalPath) == false)
            {
                Console.WriteLine("!!!RSS CONFIG FILE NOT FOUND AT:" + path);
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
            return true;
        }
        private async Task UpdateEntries()
        {
            FileStream xStream = File.Open(path.LocalPath+"_modified.xml", FileMode.OpenOrCreate);
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
                    // 2. feedFileName (nullable now)
                    /*
                    await writer.WriteStartElementAsync(null, "feedFileName", null);
                    await writer.WriteStringAsync(feed.GetFileName());
                    await writer.WriteEndElementAsync();
                    */
                    // 3. feed-url
                    await writer.WriteStartElementAsync(null, RSS_URL, null);
                    await writer.WriteCDataAsync(feed.GetURL());
                    await writer.WriteEndElementAsync();
                    // 4. expr
                    await writer.WriteStartElementAsync(null, RSS_Expression, null);
                    await writer.WriteStringAsync(feed.GetExpr());
                    await writer.WriteEndElementAsync();
                    // 5. history
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
