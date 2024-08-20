using Murong_Xue.DownloadHandling;
using Murong_Xue.Logging;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Murong_Xue
{
    internal class FeedData
    {
        protected string Title;
        protected Uri URL;
        protected string Expression;
        protected string History;
        protected string Date;
        protected bool HasNewHistory = false;
        protected string NewHistory = string.Empty;
        private static readonly DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        private static readonly Config cfg = Config.GetInstance();
        private static Reporter report = Config.OneReporterPlease("F-DATA");

        public FeedData(string title, string url,
            string expression, string history, string date)
        {
            Title = title;
            URL = new Uri(url);
            Expression = expression;
            History = history;
            Date = date;
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
            report.Spam(tmp);
        }
        public string ToLongString()
        {
            StringBuilder builder = new();
            const string sep = "----------";
            //
            builder.AppendLine(sep);
            builder.AppendLine("TITLE: " + GetTitle());
            builder.AppendLine("History: " + GetHistory());
            builder.AppendLine("Expression: " + GetExpr());
            builder.AppendLine("URL: " + GetURL());
            builder.AppendLine("Date: " + GetDate());
            builder.Append(sep); //don't end with a new line (append, not AppendLine)
            //
            return builder.ToString();
        }
        public void QueueDownload()
        {
            DownloadEntryFeed entry = new(URL, this);
            downloadHandler.QueueDownload(entry);
        }

        public void AddFile(string title, Uri link)
        {
            if (HasNewHistory == false)
            {
                HasNewHistory = true;
                NewHistory = title;
            }
            report.Out($"Add File {title} {link}");
            Uri downloadPath = new(cfg.GetDownloadPath());
            if (Path.Exists(downloadPath.LocalPath) == false)
            {
                report.Warn($"Specified download path did not exist, creating directory. {downloadPath}");
                Directory.CreateDirectory(downloadPath.LocalPath);
            }
            DownloadEntryFile entry = new(link, downloadPath);
            downloadHandler.QueueDownload(entry);
        }
        public async void OnFeedDownloaded(Stream content)
        {
            report.Notice($"Feed Downloaded len:{content.Length}, {this.Title}");
            XmlReaderSettings xSettings = new();
            xSettings.Async = false;
            DateTime lastDownload = DateTime.Parse(Date);
            //
            const string title_element = "title";
            const string link_element = "link";
            const string item_element = "item";
            const string date_element = "pubDate";
            //---
            using (XmlReader reader = XmlReader.Create(content, xSettings))
            //aynsc:avg: 00:00:00.0004290
            //sync: avg: 00:00:00.0001597
            //sync is 2.7x faster
            {
                bool IsTitle = false;
                bool IsUrl = false;
                bool IsDate = false;
                bool DateAlreadySet = false;

                string _title = string.Empty;
                string _url = string.Empty;
                string _date = string.Empty;
                DateTime tmpDate;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (reader.Name)
                            {
                                case title_element:
                                    IsTitle = true;
                                    break;
                                case link_element:
                                    IsUrl = true;
                                    break;
                                case date_element:
                                    if (DateAlreadySet == false)
                                        IsDate = true;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case XmlNodeType.Text:
                            if (IsTitle)
                            {
                                _title = reader.Value;
                                if (History == _title)
                                    return;
                            }
                            else if (IsUrl)
                            {
                                _url = reader.Value;
                            }
                            else if (IsDate)
                            {
                                _date = reader.Value;
                                tmpDate = DateTime.Parse(_date);
                                SetDate(_date.ToString());
                                if (tmpDate < lastDownload)
                                {
            /* This should solve the problem where an older version of the
            * feed looks like: 07,06,05,04,...
            * But, the newest verssion becomes 07v2,06,05,04,..
            * or even 06,05,04,..
            * Our last "History" entry would be "07", but with
            * 07 having been deleted or renamed we lose our guide for when to stop
            * However, by comparing dates we avoid this. */
                                    report.Out("OLDER DATE (TAKE NOTE!!)");
                                    return;
                                }
                                DateAlreadySet = true;
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case title_element:
                                    IsTitle = false;
                                    break;
                                case link_element:
                                    IsUrl = false;
                                    break;
                                case date_element:
                                    IsDate = false;
                                    break;
                                case item_element:
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
                report.TraceVal("GetHistory (HasNewHistory && !=empty)");
                return NewHistory;
            }
            return History;
        }
        public string GetExpr()
        {
            return Expression;
        }
        public void SetTitle(string title)
        {
            this.Title = title;
        }
        public void SetURL(string URL)
        {
            this.URL = new Uri(URL);
        }
        public void SetHistory(string History)
        {
            this.History = History;
        }
        public void SetExpr(string Expression)
        {
            this.Expression = Expression;
        }
        public string GetDate()
        {
            return this.Date;
        }
        public void SetDate(string date)
        {
            this.Date = date;
        }
    }
}
