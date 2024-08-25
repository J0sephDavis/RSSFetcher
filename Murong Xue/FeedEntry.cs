﻿using Murong_Xue.DownloadHandling;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Murong_Xue
{
    public record Feed
    {
        public int ID;
        public string Title;
        public Uri URL;
        public string Expression;
        public DateTime Date;
        public string History;

        public Feed(int id, string title, Uri url, string expression, DateTime date, string history)
        {
            ID = id;
            Title = title;
            URL = url;
            Expression = expression;
            this.Date = date;
            History = history;
        }
        public Feed(Feed copy)
        {
            ID = copy.ID;
            Title = copy.Title;
            URL = copy.URL;
            Expression = copy.Expression;
            Date = copy.Date;
            History = copy.History;
        }

        public string ToStringEntry()
        {
            return $"{ID} {Title} {Date}";
        }

        public string[] ToStringList()
        {
            return [Title, $"{Date}"];
        }
        public override string ToString()
        {
            return $"T:{Title}\tU:{URL}\tE:{Expression}\tH:{History}";
        }
    }
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
        private static readonly Config cfg = Config.GetInstance();
        private static readonly Reporter report = Logger.RequestReporter("F-DATA");
        public void Print()
        {
            report.Spam(edited == null
                ? original.ToString()
                : edited.ToString());
        }
        public string ToLongString()
        {
            StringBuilder builder = new();
            const string sep = "----------";
            //
            builder.AppendLine(sep);
            builder.AppendLine("TITLE: " + Title);
            builder.AppendLine("History: " + History);
            builder.AppendLine("Expression: " + Expression);
            builder.AppendLine("URL: " + URL);
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
            entry.Queue();
        }
        override public void HandleDownload(Stream content)
        {
            events.OnFeedDownloaded();
            report.Notice($"Feed Downloaded len:{content.Length}, {this.Title}");
            XmlReaderSettings xSettings = new();
            xSettings.Async = false;
            DateTime lastDownload = Date;
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
                Feed copy = new(original);
                string _title = string.Empty;
                string _url = string.Empty;
                bool stopReading = false; //set when we reach an entry older than our history
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
                                copy.Date = DateTime.Parse(reader.Value);
                                if (copy.Date < lastDownload)
                                {
            /* This should solve the problem where an older version of the
            * feed looks like: 07,06,05,04,...
            * But, the newest verssion becomes 07v2,06,05,04,..
            * or even 06,05,04,..
            * Our last "History" entry would be "07", but with
            * 07 having been deleted or renamed we lose our guide for when to stop
            * However, by comparing dates we avoid this. */
                                    report.Out("OLDER DATE (see if problem persists)");
                                    stopReading = true;
                                    break;
                                }
                                DateAlreadySet = true; //we only want the newest date
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case item_element:
                                    if (Regex.IsMatch(_title, this.Expression))
                                    {
                                        copy.History = _title;
                                        edited = copy;
                                        AddFile(_title, new Uri(_url));
                                    }
                                    break;
                                case title_element:
                                    IsTitle = false;
                                    break;
                                case link_element:
                                    IsUrl = false;
                                    break;
                                case date_element:
                                    IsDate = false;
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
            edited ??= new(original);
            edited.URL = new Uri(URL);
        }
    }
}
