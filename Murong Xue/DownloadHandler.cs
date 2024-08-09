using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    /* 1. Owns the HTTPClient
     * 2. 
     */
    internal sealed class DownloadHandler
    {
        private static DownloadHandler? s_DownloadHandler = null;

        private static HttpClient client = new();
        
        private List<DownloadEntryBase> Downloads //list of files to be downloaded
            = [];
        
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
            Downloads.Add(entry);
        }

        public async void ProcessDownloads()
        {
            List<Task<HttpResponseMessage>> CurrentBatch = new List<Task<HttpResponseMessage>>();
            while (Downloads.Count() != 0)
            {
                DownloadEntryBase entry = Downloads.First();
                Console.WriteLine("- entry: {0} {1}", entry.fileName, entry.link);
                Task<HttpResponseMessage> request = client.GetAsync(entry.link);
                request.ContinueWith(entry.OnDownload);
                //Add the entry to the task list & remove it from the downloads list
                CurrentBatch.Add(client.GetAsync(entry.link));
                Downloads.Remove(entry);
                
                //When we've filled our budget or used em all
                if (CurrentBatch.Count() >= 4 || Downloads.Count() == 0)
                {
                    Console.WriteLine("Downloads{0} Batch{1}", Downloads.Count(), CurrentBatch.Count());

                    await Task.WhenAll(CurrentBatch);
                    CurrentBatch.Clear();
                    await Task.Delay(2000);
                }
            }
            Console.WriteLine("ALL DOWNLOADS PROCESSED");
        }
    }

    internal class DownloadEntryBase //TODO better name?
    {
        public Uri link;
        public string fileName;
        public DownloadEntryBase(Uri link, string fileName)
        {
            this.link = link;
            this.fileName = fileName;
        }
        virtual public void OnDownload(Task<HttpResponseMessage> response)
        {
            Console.WriteLine("DownloadEntryBase.OnDownload");
        }
    }

    internal class DownloadEntryFeed : DownloadEntryBase
    {
        private FeedData feed { get; set; }
        public DownloadEntryFeed(Uri link, string fileName, FeedData _feed) : base(link, fileName)
        {
            this.feed = _feed;
        }
        override public async void OnDownload(Task<HttpResponseMessage> response)
        {
            Console.WriteLine("DownloadFeed.OnDownload");
            HttpResponseMessage msg = await response;
            string content = await msg.Content.ReadAsStringAsync();
            feed.OnFeedDownloaded(content);
        }
    }

}
