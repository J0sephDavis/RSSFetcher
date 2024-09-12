using RSSFetcher.FeedData;
using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace RSSFetcher.DownloadHandling
{
    public enum DownloadStatus
    {
        INITIALIZED,    //not queued / default
        WAITING,        //waiting to be queued
        QUEUED,         //waiting to be downloaded
        DOWNLOADING,    //waiting for download to finish
        DOWNLOADED,     //waiting for processing to begin
        PROCESSING,     //waiting for processing to end
        PROCESSED,      //done
    };
    public abstract class DownloadEntryBase
    {
        protected readonly DownloadHandler downloadHandler;
        //----
        protected Uri URL;
        protected Reporter report;
        public DownloadStatus Status { get => Getstatus(); set => SetStatus(value); }
        private object status_lock = new();
        private DownloadStatus _status = DownloadStatus.INITIALIZED;
        private DownloadStatus Getstatus()
        {
            lock (status_lock)
            {
                return _status;
            }
        }
        private void SetStatus(DownloadStatus value)
        {
            lock (status_lock)
            {
                _status = value;
            }
        }
        //By accepting the reporter we can borrow the inherited classes reporter & not reallocate
        //for each individual inherited class. For each TYPE there is one reporter, not each instance.
        public DownloadEntryBase(Uri link, DownloadHandler _dlHandle, Reporter? rep = null)
        {
            downloadHandler = _dlHandle;
            if (rep == null)
                report = Logger.RequestReporter("DLBASE");
            else
                report = rep;

            this.URL = link;
        }
        public async Task Request(HttpClient client)
        {
            Status = DownloadStatus.DOWNLOADING;

            Task<HttpResponseMessage> request = client.GetAsync(URL);
            _ = request.ContinueWith(OnDownload);
            await request; //only return when the request has actually been completed
            return;
        }
        private async Task OnDownload(Task<HttpResponseMessage> response)
        {
            Status = DownloadStatus.DOWNLOADED;
            //----------------------------------
            HttpResponseMessage msg = await response;

            if (msg.IsSuccessStatusCode == false)
            {
                if (msg.StatusCode == HttpStatusCode.TooManyRequests)
                    report.WarnSpam("Download failed, too many requests");
                else
                    report.Warn($"Download failed HTTP Status Code: {msg.StatusCode}");
                downloadHandler.ReQueue(this);
                return;
            }
            Stream content = await msg.Content.ReadAsStreamAsync();

            Status = DownloadStatus.PROCESSING;
            //----------------------------------
            _ = Task.Run(() => HandleDownload(content));
        }
        public abstract void HandleDownload(Stream content);
    }
    public class DownloadEntryFile(Uri link, DownloadHandler _dlHandle) : DownloadEntryBase(link, _dlHandle, report)
    {
        private new static readonly Reporter report = Logger.RequestReporter("DLFILE");
        override public void HandleDownload(Stream content)
        {
            Task.Run(()=> downloadHandler.FileDownloaded()); //prevent blocking of congested lock
            Uri destinationPath = new(downloadHandler.DownloadFolder + Path.GetFileName(URL.AbsolutePath));
            if (File.Exists(destinationPath.LocalPath))
                report.Error($"File already exists\n\tLink:{URL}\n\tPath:{destinationPath}");
            else using (FileStream fs = File.Create(destinationPath.LocalPath))
            {
                content.Seek(0, SeekOrigin.Begin);
                content.CopyTo(fs);
                report.Out($"FILE {URL} WRITTEN TO {destinationPath}");
            }
            Status = DownloadStatus.PROCESSED;
        }
    }
    public class DownloadEntryFeed : DownloadEntryBase
    {
        private new static readonly Reporter report = Logger.RequestReporter("DLFEED");
        //---
        private Feed feed;

        public DownloadEntryFeed(Feed _feed, DownloadHandler _dlHandle) : base(_feed.URL, _dlHandle, report)
        {
            feed = _feed;
            feed.Status |= FeedStatus.DLHANDLE;
        }

        override public void HandleDownload(Stream content)
        {
            // XML element tags
            const string title_element = "title";
            const string link_element = "link";
            const string item_element = "item";
            const string date_element = "pubDate";
            // PRE-HANDLE EVENTS
            Task.Run(()=>downloadHandler.FeedDownloaded()); //prevent blocking due to locks. summary info is secondary to finishing the downloads
            report.Notice($"Feed Downloaded len:{content.Length}, {feed.Title}");
            // ---
            XmlReaderSettings xSettings = new()
            {
                //aynsc:avg: 00:00:00.0004290
                //sync: avg: 00:00:00.0001597
                //sync is 2.7x faster
                Async = false
            };
            using XmlReader reader = XmlReader.Create(content, xSettings);

            // Control variables
            bool IsTitle = false;
            bool IsUrl = false;
            bool IsDate = false;
            bool HistoryUpdated = false;
            bool stopReading = false; //set when we reach an entry older than our history

            // Temporary Variables
            string _title = string.Empty;
            Uri? _url = null;
            DateTime _date = DateTime.UnixEpoch;

            // Memory Variables
            DateTime _originalDate = feed.Date;
            string _originalHistory = feed.History;

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
                                // saves on some cycles by not updating the date multiple times
                                if (!HistoryUpdated)
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
                                report.Trace("title matches history, stop reading.");
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
                            if (_date < _originalDate) //if the current item comes before or our last download: STOP.
                            {
                                report.Warn("OLDER DATE - this may indicate that a file was published and deleted, stop reading.");
                                stopReading = true;
                                break;
                            }
                        }
                        break;
                    case XmlNodeType.EndElement:
                        switch (reader.Name)
                        {
                            case item_element:
                                if (Regex.IsMatch(_title, feed.Expression))
                                {
                                    if (_url == null)
                                    {
                                        report.Warn($"{feed.Title} failed to add file, missing Uri");
                                        break;
                                    }
                                    //we only store the newest download (by publication date) in the history.
                                    if (!HistoryUpdated)
                                    {
                                        feed.Date = _date;
                                        feed.History = _title;
                                        feed.Status |= FeedStatus.MODIFIED;
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
            // POST-HANDLE EVENTS
            Status = DownloadStatus.PROCESSED;
        }
        private void AddFile(Uri link) //TODO add title + url to a list & retrieve later?
        {
            if (Path.Exists(downloadHandler.DownloadFolder.LocalPath) == false)
            {
                throw new NotImplementedException("this should be handleded in the DownloadHandler class when the path is set");
                //report.Warn($"Specified download path did not exist, creating directory. {downloadHandler.DownloadDir.LocalPath}");
                //Directory.CreateDirectory(downloadHandler.DownloadDir.LocalPath);
            }
            downloadHandler.AddDownload(new DownloadEntryFile(link, downloadHandler));
        }
    }
}