using System.Net;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;

namespace Murong_Xue.DownloadHandling
{
    public enum DownloadStatus
    {
        INVALID = -1, //only used in methods, never set.
        INITIALIZED,
        QUEUED,
        DOWNLOADING,
        DOWNLOADED,
        PROCESSING,
        PROCESSED,
    };
    internal abstract class DownloadEntryBase
    {
        private readonly static DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        public Uri link;
        protected Reporter report;
        static protected EventTicker events = EventTicker.GetInstance();
        public DownloadStatus status = DownloadStatus.INITIALIZED;

        //By accepting the reporter we can borrow the inherited classes reporter & not reallocate
        //for each individual inherited class. For each TYPE there is one reporter, not each instance.
        public DownloadEntryBase(Uri link, Reporter? rep = null)
        {
            this.link = link;
            if (rep == null)
                report = Logger.RequestReporter("DLBASE");
            else
                report = rep;
        }
        public void Queue()
        {
            status = DownloadStatus.QUEUED;
            //----------------------------------
            downloadHandler.QueueDownload(this);
        }
        public async Task Request(HttpClient client)
        {
            status = DownloadStatus.DOWNLOADING;

            Task<HttpResponseMessage> request = client.GetAsync(link);
            _ = request.ContinueWith(OnDownload);
            await request; //only return when the request has actually been completed
            return;
        }
        private async Task OnDownload(Task<HttpResponseMessage> response)
        {
            status = DownloadStatus.DOWNLOADED;
            //----------------------------------
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
            
            status = DownloadStatus.PROCESSING;
            //----------------------------------
            _ = Task.Run(() => HandleDownload(content));
        }
        public abstract void HandleDownload(Stream content);
        protected void DoneProcessing()
        {
            status = DownloadStatus.PROCESSED;
            //----------------------------------
            downloadHandler.RemoveProcessing(this);
        }
        private void ReQueue()
        {
            status = DownloadStatus.QUEUED;
            //----------------------------------
            downloadHandler.ReQueue(this);
        }
    }

    internal class DownloadEntryFile(Uri link, Uri DownloadPath) : DownloadEntryBase(link, Logger.RequestReporter("DLFILE"))
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