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
        private static Reporter report;


        public FeedData(string title,
            string url, string expression,
            string history, string date)
        {
            this.Title = title;
            this.URL = new Uri(url);
            this.Expression = expression;
            this.History = history;
            this.Date = date;

            report ??= Config.OneReporterPlease("FeedData");
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
            report.Trace("QueueDownload()");
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
            xSettings.Async = true;
            //
            using (XmlReader reader = XmlReader.Create(content, xSettings))
            {
                const string title_element = "title";
                const string link_element = "link";
                const string item_element = "item";
                const string date_element = "pubDate";

                bool IsTitle = false;
                bool IsUrl = false;
                bool IsDate = false;
                bool DateAlreadySet = false;

                string _title = string.Empty;
                string _url = string.Empty;
                string _date = string.Empty;

                while (await reader.ReadAsync())
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
                                _title = await reader.GetValueAsync();
                            }
                            else if (IsUrl)
                            {
                                _url = await reader.GetValueAsync();
                            }
                            else if (IsDate)
                            {
                                _date = await reader.GetValueAsync();
                                DateAlreadySet = true;
                                SetDate(DateTime.Parse(_date).ToString());
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
                report.Trace("GetHistory (HasNewHistory && !=empty)");
                return NewHistory;
            }
            report.Trace("GetHistory");
            return History;
        }
        public string GetExpr()
        {
            return Expression;
        }
        public void SetTitle(string title)
        {
            report.TraceVal($"SetTitle {title}");
            this.Title = title;
        }
        public void SetURL(string URL)
        {
            report.TraceVal($"SetURL {URL}");
            this.URL = new Uri(URL);
        }
        public void SetHistory(string History)
        {
            report.TraceVal($"SetHistory {History}");
            this.History = History;
        }
        public void SetExpr(string Expression)
        {
            report.TraceVal($"SetExpression {Expression}");
            this.Expression = Expression;
        }
        public string GetDate()
        {
            return this.Date;
        }
        public void SetDate(string date)
        {
            report.TraceVal("DATE SET:" + date);
            this.Date = date;
        }
    }
}
