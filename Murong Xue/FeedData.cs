using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Murong_Xue
{
    internal class FeedData
    {
        protected string Title;
        protected Uri URL; //TODO how to use this Uri data type
        protected string Expression;
        protected string History;
        protected bool HasNewHistory = false;
        protected string NewHistory  = string.Empty;
        private DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        private Config cfg = Config.GetInstance();
        int id = 0;

        public FeedData(string title,
            string url, string expression,
            string history)
        {
            this.Title = title;
            this.URL = new Uri(url);
            //TODO add checks on URL validity
            this.Expression = expression;
            this.History = history;
        }

        public void Print()
        {
            Console.WriteLine("FeedData Obj:" +
                $"\n\t{this.Title}" +
                $"\n\t{this.URL}" +
                $"\n\t{this.Expression}" +
                $"\n\t{this.History}");
            if (HasNewHistory)
                Console.WriteLine("\n\tNEW-HISTORY: {0}", NewHistory);
        }
        public void QueueDownload()
        {
            DownloadEntryFeed entry = new DownloadEntryFeed(URL, this);
            downloadHandler.AddDownload(entry);
        }

        public void AddFile(string title, Uri link)
        {
            if (HasNewHistory == false)
            {
                HasNewHistory = true;
                NewHistory = title;
            }
            Console.WriteLine("- NEW DOWNLOAD {0}: {1}", title, link.ToString());
            Uri downloadPath = new Uri(cfg.GetDownloadPath());
            DownloadEntryFile entry = new(link, downloadPath);
            id += 1;
            downloadHandler.AddDownload(entry);
        }
        public async void OnFeedDownloaded(Stream content)
        {
            Console.WriteLine("----- Feed Downloaded {1}({0})-----",content.Length,this.Title);
            XmlReaderSettings xSettings = new();
            xSettings.Async = true;
            //
            using (XmlReader reader = XmlReader.Create(content, xSettings))
            {
                bool IsTitle = false;
                bool IsUrl = false;
                string _title = string.Empty;
                string _url = string.Empty;

                while(reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case "title":
                                    IsTitle = true;
                                    break;
                                case "link":
                                    IsUrl = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            if (IsTitle)
                                _title = await reader.GetValueAsync();
                            else if (IsUrl)
                                _url = await reader.GetValueAsync();
                            break;
                        case XmlNodeType.EndElement:
                            switch(reader.Name)
                            {
                                case "title":
                                    IsTitle = false;
                                    break;
                                case "link":
                                    IsUrl = false;
                                    break;
                                case "item":
                                    if (History == _title)
                                        return;
                                    if (Regex.IsMatch(_title, this.Expression))
                                        AddFile(_title, new Uri(_url));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public string GetTitle()
        {
            return Title;
        }
        public string GetURL()
        {
            return URL.ToString();
        }
        public string GetHistory()
        {
            if (HasNewHistory && NewHistory != string.Empty)
            {
                return NewHistory;
            }
            return History;
        }
        public string GetExpr()
        {
            return Expression;
        }
    }
}
