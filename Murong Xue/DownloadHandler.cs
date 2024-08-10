using System;
using System.Collections.Generic;
using System.Linq;
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
        //c# version 12 does not have System.Threading.Lock, so we use Object()
        private readonly object DPLock = new(); //for when swapping between the two lists.
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
        public async Task ProcessDownloads()
        {
            List<Task> CurrentBatch = [];
            List<DownloadEntryBase> tmpList = [];
            while (Downloads.Count != 0 || Processing.Count != 0)
            {
                while (Downloads.Count != 0)
                {
                    DownloadEntryBase entry = PopSwapDownload();
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
        public void AddDownload(DownloadEntryBase entry)
        {
            lock(DPLock)
            {
                Downloads.Add(entry);
            }
        }
        private DownloadEntryBase PopSwapDownload()
        {
            DownloadEntryBase? entry = null;
            lock (DPLock)
            {
                entry = Downloads.First();
                Downloads.Remove(entry);
                Processing.Add(entry);
            }
            return entry;
        }
        public void RemoveProcessing(DownloadEntryBase entry)
        {
            lock(DPLock)
            {
                Processing.Remove(entry);
            }
        }
    }
}
