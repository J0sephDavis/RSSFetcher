using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal class DownloadEntryBase //TODO better name?
    {
        protected DownloadHandler downloadHandler = DownloadHandler.GetInstance();
        public Uri link;
        public DownloadEntryBase(Uri link)
        {
            this.link = link;
        }
        virtual public async Task Request(HttpClient client)
        {
            Task<HttpResponseMessage> request = client.GetAsync(this.link);
            await request.ContinueWith(this.OnDownload);
            await request; //only return when the request has actually been completed
            return;
        }
        virtual public async void OnDownload(Task<HttpResponseMessage> response)
        {
            Console.WriteLine("DownloadEntryBase.OnDownload");
            RemoveProcessing();
        }
        protected void RemoveProcessing()
        {
            downloadHandler.RemoveProcessing(this);
        }
    }

    internal class DownloadEntryFeed : DownloadEntryBase
    {
        private FeedData feed { get; set; }
        public DownloadEntryFeed(Uri link, FeedData _feed)
            : base(link)
        {
            this.feed = _feed;
        }
        override public async void OnDownload(Task<HttpResponseMessage> response)
        {
            HttpResponseMessage msg = await response;
            Stream content = await msg.Content.ReadAsStreamAsync();
            feed.OnFeedDownloaded(content);
            RemoveProcessing();
        }
    }

    internal class DownloadEntryFile : DownloadEntryBase
    {
        private Uri DownloadPath;
        public DownloadEntryFile(Uri link, Uri DownloadPath)
            : base(link)
        {
            this.DownloadPath = DownloadPath;
        }
        override public async void OnDownload(Task<HttpResponseMessage> response)
        {
            Console.WriteLine("File downloaded: " + link.ToString());
            HttpResponseMessage resp = await response;

            Stream content = await resp.Content.ReadAsStreamAsync();
            if (File.Exists(this.DownloadPath.ToString()))
            {
                Console.WriteLine("file already exists {0}", this.DownloadPath);
                return;
            }
            using (FileStream fs = File.Create(this.DownloadPath.ToString()))
            {
                content.Seek(0, SeekOrigin.Begin);
                content.CopyTo(fs);
            }
            Console.WriteLine("FILE WRITTEN TO {0}", this.DownloadPath);
            RemoveProcessing();
        }
    }
}
