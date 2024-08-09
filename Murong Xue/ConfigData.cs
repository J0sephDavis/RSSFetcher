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
    internal class ConfigData
    {
        private Uri path;
        private List<FeedData> data;
        private DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        public ConfigData(Uri filePath)
        {
            this.path = filePath;
            data = new List<FeedData>();
            Console.WriteLine(
                $"filePath: IsFile?{filePath.IsFile}" +
                $"LocalPath:{filePath.LocalPath}"
            );
        }
        public async Task Process()
        {
            GetConfigData();
            //
            foreach(FeedData feed in data)
                feed.QueueDownload();
            //
            await downloadHandler.ProcessDownloads();
        }
        //! Reads the config XML files and populated the FeedData list
        private void GetConfigData()
        {
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
                                    data.Add(new FeedData(
                                        title:      _title,
                                        fileName:   _fileName,
                                        url:        _url,
                                        expression: _expr,
                                        history:    _history)
                                    );
                                    data.Last().Print();
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
        }
    }
}
