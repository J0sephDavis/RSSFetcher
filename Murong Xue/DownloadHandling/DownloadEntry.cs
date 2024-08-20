using System.Net;
using Murong_Xue.Logging;

namespace Murong_Xue.DownloadHandling
{
    internal abstract class DownloadEntryBase
    {
        private static DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        public Uri link;
        protected Reporter report;
        static protected EventTicker events = EventTicker.GetInstance();
        public DownloadEntryBase(Uri link, Reporter? rep = null)
        {
            this.link = link;
            if (rep == null)
                report = Config.OneReporterPlease("DLBASE");
            else
                report = rep;
        }
        public void Queue()
        {
            downloadHandler.QueueDownload(this);
        }
        public async Task Request(HttpClient client)
        {
            Task<HttpResponseMessage> request = client.GetAsync(link);
            _ = request.ContinueWith(OnDownload);
            await request; //only return when the request has actually been completed
            return;
        }
        private async Task OnDownload(Task<HttpResponseMessage> response)
        {
            HttpResponseMessage msg = await response;

            if (msg.IsSuccessStatusCode == false)
            {
                if (msg.StatusCode == HttpStatusCode.TooManyRequests)
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
        public abstract void HandleDownload(Stream content);
        protected void DoneProcessing()
        {
            downloadHandler.RemoveProcessing(this);
        }
        private void ReQueue()
        {
            downloadHandler.ReQueue(this);
        }
    }

    internal class DownloadEntryFile(Uri link, Uri DownloadPath) : DownloadEntryBase(link, Config.OneReporterPlease("DLFILE"))
    {
        override public void HandleDownload(Stream content)
        {
            events.OnFileDownloaded();
            string fileName = Path.GetFileName(link.AbsolutePath);
            string destinationPath = DownloadPath.LocalPath + fileName;
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