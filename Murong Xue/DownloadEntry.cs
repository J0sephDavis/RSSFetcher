using System.Net;

namespace Murong_Xue
{
    internal abstract class DownloadEntryBase
    {
        protected DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        public Uri link;
        protected Reporter report;
        public DownloadEntryBase(Uri link, string reportIdentifier = "DownloadEntryBase")
        {
            this.link = link;
            report ??= Config.OneReporterPlease(reportIdentifier);
        }
        public async Task Request(HttpClient client)
        {
            report.Log(LogFlag.DEBUG_SPAM, "Requesting data");
            Task<HttpResponseMessage> request = client.GetAsync(this.link);
            _ = request.ContinueWith(this.OnDownload);
            await request; //only return when the request has actually been completed
            return;
        }
        public abstract void HandleDownload(Stream content);
        protected void DoneProcessing()
        {
            downloadHandler.RemoveProcessing(this);
        }
        private async Task OnDownload(Task<HttpResponseMessage> response)
        {
            report.Log(LogFlag.DEBUG_SPAM, "On Download");

            HttpResponseMessage msg = await response;
                
            if (msg.IsSuccessStatusCode  == false)
            {
                if (msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    report.Log(LogFlag.DEBUG_SPAM | LogFlag.WARN, $"Download failed, too many requests");
                else
                    report.Log(LogFlag.ERROR, $"Download failed HTTP Status Code: {msg.StatusCode}");
                ReQueue();
                return;
            }

            Stream content = await msg.Content.ReadAsStreamAsync();
            SetProcessing();
            _ = Task.Run(() => HandleDownload(content));

            report.Log(LogFlag.ERROR, $"HTTP Request Exception: {link} {e.Message}");
        }
        private void SetProcessing()
        {
            report.Log(LogFlag.DEBUG_SPAM, "Remove Processing");
            downloadHandler.DownloadingToProcessing(this);
        }
        private void ReQueue()
        {
            report.Log(LogFlag.DEBUG_SPAM, "ReQueue");
            downloadHandler.ReQueue(this);
        }
    }

    internal class DownloadEntryFeed : DownloadEntryBase
    {
        private FeedData Feed { get; set; }
        public DownloadEntryFeed(Uri link, FeedData _feed) : base(link, "DownloadEntryFeed")
        {
            this.Feed = _feed;
        }
        override public async void HandleDownload(Stream content)
        {
            report.Log(LogFlag.DEBUG_SPAM, "Handle Download");
            await Task.Run(() => Feed.OnFeedDownloaded(content));
            DoneProcessing();
        }
    }

    internal class DownloadEntryFile : DownloadEntryBase
    {
        private readonly Uri DownloadPath;
        public DownloadEntryFile(Uri link, Uri DownloadPath) : base(link, "DownloadEntryFile")
        {
            this.DownloadPath = DownloadPath;
        }
        override public void HandleDownload(Stream content)
        {
            report.Log(LogFlag.DEBUG_SPAM, $"Handle Download {link}");
            string fileName = Path.GetFileName(link.AbsolutePath);
            string destinationPath = this.DownloadPath.LocalPath + fileName;
            if (File.Exists(destinationPath))
                report.Log(LogFlag.ERROR, $"File already exists {link} {destinationPath}");
            else using (FileStream fs = File.Create(destinationPath))
            {
                content.Seek(0, SeekOrigin.Begin);
                content.CopyTo(fs);
                report.Log(LogFlag.NOTEWORTHY, $"FILE {link} WRITTEN TO {destinationPath}");
            }
            DoneProcessing();
        }
    }
}