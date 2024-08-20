using System.Net;

namespace Murong_Xue
{
    internal abstract class DownloadEntryBase
    {
        protected DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        public Uri link;
        protected Reporter report;
        static protected EventTicker events = EventTicker.GetInstance();
        public DownloadEntryBase(Uri link, string reportIdentifier = "DLBASE")
        {
            this.link = link;
            report ??= Config.OneReporterPlease(reportIdentifier);
        }
        public async Task Request(HttpClient client)
        {
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
            HttpResponseMessage msg = await response;
                
            if (msg.IsSuccessStatusCode  == false)
            {
                if (msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    report.WarnSpam("Download failed, too many requests");
                else
                    report.Warn($"Download failed HTTP Status Code: {msg.StatusCode}");
                ReQueue();
                return;
            }
            Stream content = await msg.Content.ReadAsStreamAsync();
            SetProcessing();
            _ = Task.Run(() => HandleDownload(content));
        }
        private void SetProcessing()
        {
            downloadHandler.DownloadingToProcessing(this);
        }
        private void ReQueue()
        {
            downloadHandler.ReQueue(this);
        }
    }

    internal class DownloadEntryFeed : DownloadEntryBase
    {
        private FeedData Feed { get; set; }
        public DownloadEntryFeed(Uri link, FeedData _feed) : base(link, "DLFEED")
        {
            this.Feed = _feed;
        }
        override public async void HandleDownload(Stream content)
        {
            events.OnFeedDownloaded();
            await Task.Run(() => Feed.OnFeedDownloaded(content));
            DoneProcessing();
        }
    }

    internal class DownloadEntryFile : DownloadEntryBase
    {
        private readonly Uri DownloadPath;
        public DownloadEntryFile(Uri link, Uri DownloadPath) : base(link, "DLFILE")
        {
            this.DownloadPath = DownloadPath;
        }
        override public void HandleDownload(Stream content)
        {
            events.OnFileDownloaded();
            string fileName = Path.GetFileName(link.AbsolutePath);
            string destinationPath = this.DownloadPath.LocalPath + fileName;
            if (File.Exists(destinationPath))
                report.Error($"File already exists\n\tLink:{link}\n\tPath:{destinationPath}");
            else using (FileStream fs = File.Create(destinationPath))
            {
                content.Seek(0, SeekOrigin.Begin);
                content.CopyTo(fs);
                report.Out($"FILE {link} WRITTEN TO {destinationPath}");
            }
            DoneProcessing();
        }
    }
}