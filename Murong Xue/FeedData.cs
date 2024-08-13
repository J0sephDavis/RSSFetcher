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
        private readonly DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        private readonly Config cfg = Config.GetInstance();
        private readonly Reporter report;
  

        public FeedData(string title,
            string url, string expression,
            string history)
        {
            this.Title = title;
            this.URL = new Uri(url); //TODO add checks on URL validity
            this.Expression = expression;
            this.History = history;

            report = Config.OneReporterPlease("FeedData");
        }

        public void Print()
        {
            string tmp = "FeedData Obj:" +
                $"\n\t{this.Title}" +
                $"\n\t{this.URL}" +
                $"\n\t{this.Expression}" +
                $"\n\t{this.History}";
            if (HasNewHistory)
                tmp += $"\n\tNEW-HISTORY: {NewHistory}";
            report.Log(LogFlag.NOTEWORTHY | LogFlag.SPAM, tmp);
        }
        public void QueueDownload()
        {
            report.Log(LogFlag.DEBUG_SPAM, "Queue Download");
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
            report.Log(LogFlag.FEEDBACK, $"Add File {title} {link.ToString()}");
            Uri downloadPath = new Uri(cfg.GetDownloadPath());
            DownloadEntryFile entry = new(link, downloadPath);
            downloadHandler.AddDownload(entry);
        }
        public async void OnFeedDownloaded(Stream content)
        {
            report.Log(LogFlag.NOTEWORTHY, $"Feed Downloaded len:{content.Length}, {this.Title}");
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
                report.Log(LogFlag.DEBUG_SPAM, "GetHistory (HasNewHistory && !=empty)");
                return NewHistory;
            }
            report.Log(LogFlag.DEBUG_SPAM, "GetHistory");
            return History;
        }
        public string GetExpr()
        {
            return Expression;
        }
        public void SetTitle(string title)
        {
            report.Log(LogFlag.DEBUG_SPAM, $"SetTitle {title}");
            this.Title = title;
        }
        public void  SetURL(string URL)
        {
            report.Log(LogFlag.DEBUG_SPAM, $"SetURL {URL}");
            //TODO do some checks here to make sure it is a valid URL & catch any errors
            this.URL = new Uri(URL);
        }
        public void SetHistory(string History)
        {
            report.Log(LogFlag.DEBUG_SPAM, $"SetHistory {History}");
            this.History = History;
        }
        public void SetExpr(string Expression)
        {
            report.Log(LogFlag.DEBUG_SPAM, $"SetExpression {Expression}");
            this.Expression = Expression;
        }
    }
}
