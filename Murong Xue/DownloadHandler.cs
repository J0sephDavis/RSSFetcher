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
}
