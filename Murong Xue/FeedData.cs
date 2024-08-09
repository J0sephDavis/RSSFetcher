﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal class FeedData
    {
        enum Status
        {
            Initialized,
            Queued,
            Downloaded,
            Processed
        };
        protected string Title;
        protected string FileName;
        protected Uri URL; //TODO how to use this Uri data type
        protected string Expression;
        protected string History;
        protected bool HasNewHistory = false;
        protected string NewHistory;
        private DownloadHandler downloadHandler;
        private Status _status;

        public FeedData(string title, string fileName,
            string url, string expression,
            string history, DownloadHandler downloadHandler)
        {
            this.Title = title;
            this.FileName = fileName;
            this.URL = new Uri(url);
            //TODO add checks on URL validity
            this.Expression = expression;
            this.History = history;
            this.downloadHandler = downloadHandler;
            this._status = Status.Initialized;
        }

        public void Print()
        {
            Console.WriteLine("FeedData Obj:" +
                $"\n\t{this.Title}" +
                $"\n\t{this.FileName}" +
                $"\n\t{this.URL}" +
                $"\n\t{this.Expression}" +
                $"\n\t{this.History}");
            if (HasNewHistory)
                Console.WriteLine("\n\tNEW-HISTORY: {0}", NewHistory);
        }
        /*  1. Download the file
            2. Read the file
                2.1. If history != title
                    2.1.1. If (HasNewHistory == false) //only adds the newest/first item to NewHistory
                        HasNewHistory = true
                        NewHistory = title
                    2.1.2. Add URI to downloads
                2.2. else RETURN
        */
        public void QueueDownload()
        {
            if (_status >= Status.Queued)
            {
                Console.WriteLine("File has already been enqueued");
                return;
            }

            DownloadEntryFeed entry = new DownloadEntryFeed(URL, FileName, this);
            downloadHandler.AddDownload(entry);
            _status = Status.Queued;
        }
        public void OnFeedDownloaded(string content)
        {
            _status = Status.Downloaded;
            Console.WriteLine("----- Feed Downloaded {0}-----",content.Length);
        }
    }
}
