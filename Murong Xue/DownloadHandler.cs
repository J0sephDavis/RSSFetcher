using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal sealed class DownloadHandler
    {
        const int BATCH_SIZE = 5;
        const int BATCH_DELAY_MS = 2000;
        private static DownloadHandler? s_DownloadHandler = null;
        private static HttpClient client = new();
        private readonly object DownloadsLock = new(); //c# version 12 has not System.Threading.Lock
        //list of files to be downloaded
        private List<DownloadEntryBase> Downloads = [];
        
        private DownloadHandler()
        { }

        public static DownloadHandler GetInstance()
        {
            if (s_DownloadHandler == null)
                s_DownloadHandler = new DownloadHandler();
            return s_DownloadHandler;
        }

        public void AddDownload(DownloadEntryBase entry)
        {
            lock(DownloadsLock)
            {
                Downloads.Add(entry);
            }
        }
        private DownloadEntryBase PopDownload()
        {
            DownloadEntryBase entry;
            lock (DownloadsLock)
            {
                entry = Downloads.First();
                Downloads.Remove(entry);
            }
            return entry;
        }
        public async void ProcessDownloads()
        {
            List<Task> CurrentBatch = [];
            List<DownloadEntryBase> tmpList = [];
            while (Downloads.Count != 0)
            {
                DownloadEntryBase entry = PopDownload();
                //Add the entry to the task list
                CurrentBatch.Add(entry.Request(client));
                
                //When we've filled our budget or used em all
                if (CurrentBatch.Count >= BATCH_SIZE || Downloads.Count == 0)
                {
                    Console.WriteLine("Downloads{0} Batch{1}", Downloads.Count, CurrentBatch.Count);

                    await Task.WhenAll(CurrentBatch);
                    CurrentBatch.Clear();
                    await Task.Delay(BATCH_DELAY_MS);
                }
            }
            Console.WriteLine("ALL DOWNLOADS PROCESSED");
        }
    }

    internal class DownloadEntryBase //TODO better name?
    {
        public Uri link;
        public DownloadEntryBase(Uri link)
        {
            this.link = link;
        }
        virtual public async Task Request(HttpClient client)
        {
            Task<HttpResponseMessage> request = client.GetAsync(this.link);
            await request.ContinueWith(this.OnDownload);
            return;
        }
        virtual public async void OnDownload(Task<HttpResponseMessage> response)
        {
            Console.WriteLine("DownloadEntryBase.OnDownload");
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

            //set/check state of the ProcessDownloads functions so we know whether we should start it again?
            //check if response size is low / equal to a known rate limit value, add back to the queue?
            //handle in FeedData?

            feed.OnFeedDownloaded(content);
        }
    }

    internal class DownloadEntryFile : DownloadEntryBase
    {
        public DownloadEntryFile(Uri link)
            : base(link)
        { }
        override public async void OnDownload(Task<HttpResponseMessage> response)
        {
            Console.WriteLine("File downloaded: " + link.ToString());
            HttpResponseMessage resp = await response;
        }

    }
}
