using Murong_Xue.DownloadHandling;
using Murong_Xue.Logging;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Murong_Xue
{
    internal class FeedEntry(string Title, Uri URL,
        string Expression, string History, string Date) : DownloadEntryBase(URL, report)
    {
        public string Title { get; set; } = Title;
        protected Uri URL = URL;
        public string Expression { get; set; } = Expression;
        public string History { get; set; } = History;
        public string Date { get; set; } = Date;
        protected bool HasNewHistory { get; set; } = false;
        protected string NewHistory = string.Empty;
        //From putting a debug point on the below functions. It seems that a primary constructor
        //does not redeclare these static variables, thankfully. I assume static is done at runtime?
        //Will need more research done
        private static readonly DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        private static readonly Config cfg = Config.GetInstance();
        private static readonly Reporter report = Config.OneReporterPlease("F-DATA");
        public void Print()
        {
            string tmp = "FeedEntry Obj:" +
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
            builder.AppendLine("TITLE: " + Title);
            builder.AppendLine("History: " + GetHistory());
            builder.AppendLine("Expression: " + Expression);
            builder.AppendLine("URL: " + GetURL());
            builder.AppendLine("Date: " + Date);
            builder.Append(sep); //don't end with a new line (append, not AppendLine)
            //
            return builder.ToString();
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
        override public void HandleDownload(Stream content)
        {
            events.OnFeedDownloaded();
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
                bool stopReading = false;
                while (!stopReading && reader.Read())
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
                                {
                                    stopReading = true;
                                    break;
                                }
                            }
                            else if (IsUrl)
                            {
                                _url = reader.Value;
                            }
                            else if (IsDate)
                            {
                                _date = reader.Value;
                                tmpDate = DateTime.Parse(_date);
                                Date = _date.ToString();
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
                                    stopReading = true;
                                    break;
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
                //----
                DoneProcessing();
            }
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
        public void SetURL(string URL)
        {
            this.URL = new Uri(URL);
        }
    }
}
