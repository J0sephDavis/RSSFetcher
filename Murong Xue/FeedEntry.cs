using Murong_Xue.DownloadHandling;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Murong_Xue
{
    /* feed status flags should answer:
     1. During what phase/when was the feed created (by user in UI or loaded from file, or ?)
     2. Has the object been replicated on both sides of the boundary?
     3. Is the object currently in the process of being downloaded? (queued, processed, handling,&c)
     4. Is the feed incomplete? Missing URL/Title?
     5. Is the feed slated for deletion on save+quit?
    */
    [Flags]
    public enum FeedStatus
    {
        INIT = 0, //only used in constructor
        LINKED = 1 << 0, //has a FeedEntry associated with it.
        UPDATED = 1 << 1, //updated during processing
    }
    public record Feed
    {
        public int ID;
        public string Title;
        public Uri? URL;
        public string Expression;
        public DateTime Date;
        public string History;
        public FeedStatus Status;
        public DownloadStatus? dStatus;
        public Feed() //null constructor
        {
            ID = -1;
            Title = string.Empty;
            URL = null;
            Expression = string.Empty;
            Date = DateTime.UnixEpoch;
            History = string.Empty;
            dStatus = null;
        }
        public Feed(Feed copy)
        {
            ID = copy.ID;
            Title = copy.Title;
            URL = copy.URL;
            Expression = copy.Expression;
            Date = copy.Date;
            History = copy.History;
            Status = copy.Status;
            dStatus = copy.dStatus;
        }
        public override string ToString()
        {
            return $"T:{Title}\tU:{URL}\tE:{Expression}\tH:{History}";
        }
    }
    internal class FeedEntry : DownloadEntryBase
    {
        //------------------Accessing Records-----------------------------------------------------
        private readonly Feed feed;
        public int ID { get => feed.ID; }
        public string Title { get => feed.Title; set => feed.Title = value; }
        public string Expression { get => feed.Expression; set => feed.Expression = value; }
        public DateTime Date { get => feed.Date; set => feed.Date = value; }
        public string History { get => feed.History; set => feed.History = value; }
        public Uri? URL { get => feed.URL; set => feed.URL = value; }
        public DownloadStatus? StatusDL { get => base.status; }
        public Feed GetFeed() { return feed; }
        //----------------------------------------------------------------------------------------
        private static readonly Config cfg = Config.GetInstance();
        private new static readonly Reporter report = Logger.RequestReporter("F-DATA");

        public FeedEntry(Feed _feed) : base(_feed?.URL, report)
        {
            feed = _feed;
            feed.dStatus = base.status;
        }

        //----------------------------------------------------------------------------------------
        public void Print()
        {
            report.Spam(feed.ToString());
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
        public void AddFile(Uri link)
        {
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
                bool HistoryUpdated = false;
                
                string _title = string.Empty;
                Uri? _url = null;
                DateTime _date = DateTime.UnixEpoch;
                //feed.Date & feed.History are set when a file is added.
                DateTime _originalDate = Date;
                string _originalHistory = History;

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
                                    if(!HistoryUpdated) // saves on some cycles by not updating the date multiple times
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
                                if (_originalHistory == _title)
                                {
                                    stopReading = true;
                                    break;
                                }
                            }
                            else if (IsUrl)
                            {
                                _url = new(reader.Value);
                            }
                            else if (IsDate)
                            {
                                _date = DateTime.Parse(reader.Value);
                                //entry date 
                                if (_date < _originalDate) //if the current item comes before or our last download: STOP.
                                {
            /* This should solve the problem where an older version of the
            * feed looks like: 07,06,05,04,...
            * But, the newest verssion becomes 07v2,06,05,04,..
            * or even 06,05,04,..
            * Our last "History" entry would be "07", but with
            * 07 having been deleted or renamed we lose our guide for when to stop
            * However, by comparing dates we avoid this. */
                                    report.Warn("OLDER DATE (see if problem persists)");
                                    stopReading = true;
                                    break;
                                }
                            }
                            break;
                        case XmlNodeType.EndElement:
                            switch (reader.Name)
                            {
                                case item_element:
                                    if (Regex.IsMatch(_title, this.Expression))
                                    {
                                        if (_url == null)
                                        {
                                            report.Warn($"{feed.Title} failed to add file, missing Uri");
                                            break;
                                        }
                                        //we only store the newest download (by publication date) in the history.
                                        if (!HistoryUpdated)
                                        {
                                            Date = _date;
                                            History = _title;
                                            feed.Status |= FeedStatus.UPDATED;
                                            HistoryUpdated = true;
                                        }
                                        report.Out($"({feed.Title}) Add File {_title}\t{base.URL}");
                                        AddFile(_url);
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
    }
}
