using System.Net;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;

namespace Murong_Xue.DownloadHandling
{
    public enum DownloadStatus
    {
        INITIALIZED,    //just created
        WAITING,        //waiting to be queued
        QUEUED,         //waiting to be downloaded
        DOWNLOADING,    //waiting for download to finish
        DOWNLOADED,     //waiting for processing to begin
        PROCESSING,     //waiting for processing to end
        PROCESSED,      //done
    };
    internal abstract class DownloadEntryBase
    {
        private static readonly DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        protected static readonly Config cfg = Config.GetInstance();
        protected static readonly EventTicker events = EventTicker.GetInstance();
        //----
        protected Uri URL;
        protected Reporter report;
        public DownloadStatus status = DownloadStatus.INITIALIZED; //ensuring default is 0

        //By accepting the reporter we can borrow the inherited classes reporter & not reallocate
        //for each individual inherited class. For each TYPE there is one reporter, not each instance.
        public DownloadEntryBase(Uri link, Reporter? rep = null)
        {
            if (rep == null)
                report = Logger.RequestReporter("DLBASE");
            else
                report = rep;
            
            this.URL = link;
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

            Task<HttpResponseMessage> request = client.GetAsync(URL);
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
    internal class DownloadEntryFile(Uri link, Uri DownloadPath) : DownloadEntryBase(link, report)
    {
        private new static readonly Reporter report = Logger.RequestReporter("DLFILE");
        override public void HandleDownload(Stream content)
        {
            events.OnFileDownloaded();
            string fileName = Path.GetFileName(URL.AbsolutePath);
            string destinationPath = DownloadPath.LocalPath + fileName;
            if (File.Exists(destinationPath))
                report.Error($"File already exists\n\tLink:{URL}\n\tPath:{destinationPath}");
            else using (FileStream fs = File.Create(destinationPath))
                {
                    content.Seek(0, SeekOrigin.Begin);
                    content.CopyTo(fs);
                    report.Out($"FILE {URL} WRITTEN TO {destinationPath}");
                }
            DoneProcessing();
            DoneProcessing();
        }
    }
}