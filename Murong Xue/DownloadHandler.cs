using System;
using System.Collections.Generic;
using System.Linq;
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
            = new List<DownloadEntryBase>();
        
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
            foreach (DownloadEntryBase entry in Downloads)
            {
                Console.WriteLine("- entry: {0} {1}", entry.fileName, entry.link);
                using HttpResponseMessage response = await client.GetAsync(entry.link);
                response.EnsureSuccessStatusCode();
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine("RECEIVED:{0}", content);
                
                entry.OnDownload();
            }
        }
    }

    internal class DownloadEntryBase //TODO better name?
    {
        public Uri link = null;
        public string fileName = string.Empty;
        public DownloadEntryBase(Uri link, string fileName)
        {
            this.link = link;
            this.fileName = fileName;
        }
        virtual public void OnDownload()
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
        override public void OnDownload()
        {
            Console.WriteLine("DownloadFeed.OnDownload");
            feed.OnFeedDownloaded();
        }
    }

}
