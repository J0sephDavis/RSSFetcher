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
                string _fileName = string.Empty;
                string _url = string.Empty;
                string _expr = string.Empty;
                string _history = string.Empty;

                bool InTitle = false;
                bool InFileName = false;
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
                                case "title":
                                    InTitle = true;
                                    break;
                                case "feedFileName":
                                    InFileName = true;
                                    break;
                                case "feed-url":
                                    InUrl = true;
                                    break;
                                case "expr":
                                    InExpr = true;
                                    break;
                                case "history":
                                    InHistory = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            if (InTitle)
                                _title = reader.Value;
                            else if (InFileName)
                                _fileName = reader.Value;
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
                                case "title":
                                    InTitle = false;
                                    break;
                                case "feedFileName":
                                    InFileName = false;
                                    break;
                                case "feed-url":
                                    InUrl = false;
                                    break;
                                case "expr":
                                    InExpr = false;
                                    break;
                                case "history":
                                    InHistory = false;
                                    break;
                                case "item":
                                    Feeds.Add(new FeedData(
                                        title:      _title,
                                        fileName:   _fileName,
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
                    await writer.WriteStartElementAsync(null, "item", null);
                    // 1. Title
                    await writer.WriteStartElementAsync(null, "tite", null);
                    await writer.WriteStringAsync(feed.GetTitle());
                    await writer.WriteEndElementAsync();
                    // 2. feedFileName (nullable now)
                    /*
                    await writer.WriteStartElementAsync(null, "feedFileName", null);
                    await writer.WriteStringAsync(feed.GetFileName());
                    await writer.WriteEndElementAsync();
                    */
                    // 3. feed-url
                    await writer.WriteStartElementAsync(null, "feed-url", null);
                    await writer.WriteCDataAsync(feed.GetURL());
                    await writer.WriteEndElementAsync();
                    // 4. expr
                    await writer.WriteStartElementAsync(null, "expr", null);
                    await writer.WriteStringAsync(feed.GetExpr());
                    await writer.WriteEndElementAsync();
                    // 5. history
                    await writer.WriteStartElementAsync(null, "history", null);
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
