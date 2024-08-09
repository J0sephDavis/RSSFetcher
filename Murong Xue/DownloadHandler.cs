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
        const int BATCH_DELAY_MS = 100;
        private static DownloadHandler? s_DownloadHandler = null;
        private static HttpClient client = new();
        private readonly object DownloadsLock = new(); //c# version 12 has not System.Threading.Lock
        private readonly object ProcessingLock = new();
        //list of files to be downloaded
        private List<DownloadEntryBase> Downloads = [];
        //Not in download, but also not done.
        private List<DownloadEntryBase> Processing = [];
        
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
        public void AddProcessing(DownloadEntryBase entry)
        {
            lock(ProcessingLock)
            {
                Processing.Add(entry);
            }
        }
        public void RemoveProcessing(DownloadEntryBase entry)
        {
            lock(ProcessingLock)
            {
                Processing.Remove(entry);
            }
        }
        public async Task ProcessDownloads()
        {
            List<Task> CurrentBatch = [];
            List<DownloadEntryBase> tmpList = [];
            while(Downloads.Count != 0 || Processing.Count != 0)
            {
                Console.WriteLine("!!!!!! D{0} P{1}", Downloads.Count, Processing.Count);
                while (Downloads.Count != 0)
                {
                    DownloadEntryBase entry = PopDownload();
                    //Add the entry to the task list
                    CurrentBatch.Add(entry.Request(client));
                    //When we've filled our budget or used em all
                    if (CurrentBatch.Count >= BATCH_SIZE || Downloads.Count == 0)
                    {
                        Console.WriteLine("Downloads{0} Batch{1} Processing{2}", Downloads.Count, CurrentBatch.Count, Processing.Count);
                        await Task.WhenAll(CurrentBatch);
                        CurrentBatch.Clear();
                        await Task.Delay(BATCH_DELAY_MS);
                    }
                }
            }
            Console.WriteLine("ALL DOWNLOADS PROCESSED");
        }
    }

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
            downloadHandler.AddProcessing(this);
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

            //set/check state of the ProcessDownloads functions so we know whether we should start it again?
            //check if response size is low / equal to a known rate limit value, add back to the queue?
            //handle in FeedData?

            feed.OnFeedDownloaded(content);
            RemoveProcessing();
        }
    }

    internal class DownloadEntryFile : DownloadEntryBase
    {
        private string DownloadPath = string.Empty;
        public DownloadEntryFile(Uri link, string DownloadPath)
            : base(link)
        {
            this.DownloadPath = DownloadPath;
        }
        override public async void OnDownload(Task<HttpResponseMessage> response)
        {
            Console.WriteLine("File downloaded: " + link.ToString());
            HttpResponseMessage resp = await response;
            
            Stream content = await resp.Content.ReadAsStreamAsync();
            if (File.Exists(this.DownloadPath))
            {
                Console.WriteLine("file already exists {0}", this.DownloadPath);
                return;
            }
            using (FileStream fs = File.Create(this.DownloadPath))
            {
                content.Seek(0, SeekOrigin.Begin);
                content.CopyTo(fs);
            }
            Console.WriteLine("FILE WRITTEN TO {0}", this.DownloadPath);
            RemoveProcessing();
        }

    }
}
