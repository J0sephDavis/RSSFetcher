namespace Murong_Xue
{
    internal abstract class DownloadEntryBase
    {
        protected DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        public Uri link;
        protected Reporter report;
        public DownloadEntryBase(Uri link)
        {
            this.link = link;
            report ??= Config.OneReporterPlease("DownloadEntryBase");
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
            try
            {
                HttpResponseMessage msg = await response;
                msg.EnsureSuccessStatusCode();

                Stream content = await msg.Content.ReadAsStreamAsync();
                SetProcessing();
                _ = Task.Run(() => HandleDownload(content));
            }
            catch (HttpRequestException e)
            {
                report.Log(LogFlag.ERROR, $"HTTP Request Exception: {e.Message}");
                ReQueue();
            }
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
        public DownloadEntryFeed(Uri link, FeedData _feed) : base(link)
        {
            this.Feed = _feed;
            report ??= Config.OneReporterPlease("DownloadEntryFeed");
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
        public DownloadEntryFile(Uri link, Uri DownloadPath) : base(link)
        {
            this.DownloadPath = DownloadPath;
            report ??= Config.OneReporterPlease("DownloadEntryFile");
        }
        override public void HandleDownload(Stream content)
        {
            report.Log(LogFlag.DEBUG_SPAM, "Handle Download");
            string fileName = Path.GetFileName(link.AbsolutePath);
            string destinationPath = this.DownloadPath.LocalPath + fileName;
            if (File.Exists(destinationPath))
                report.Log(LogFlag.ERROR, $"File already exists {destinationPath}");
            else using (FileStream fs = File.Create(destinationPath))
            {
                content.Seek(0, SeekOrigin.Begin);
                content.CopyTo(fs);
                report.Log(LogFlag.NOTEWORTHY, $"FILE WRITTEN TO {destinationPath}");
            }
            DoneProcessing();
        }
    }
}