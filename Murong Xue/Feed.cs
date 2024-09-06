using System.Text;

namespace Murong_Xue
{
    /* feed status flags should answer:
     1. During what phase/when was the feed created (by user in UI or loaded from file, or ?)
     2. Has the object been replicated on both sides of the boundary?
     3. Is the object currently in the process of being downloaded? (queued, processed, handling,&c)
     4. Is the feed incomplete? Missing URL/Title?
     5. Is the feed slated for deletion on save+quit?
    */
    [Flags]
    public enum FeedStatus
    {
        INIT = 0,
        // Represented in FeedManager (not a free floating feed)
        LINKED = 1 << 0,
        // added to download queue at some point
        // (says nothing about whether its waiting/downloading/processing/done)
        DLHANDLE = 1 << 1,
        // modified by user/system
        MODIFIED = 1 << 2,
        // Added from config file T/F. F->Created by user during session
        FROM_FILE = 1 << 3,
        // STUB (not ready / not populated with data), should just be an ID
        STUB = 1 << 4,
        //SAVE/DISCARD
    }
    public record Feed
    {
        public int ID               = -1;
        public string Title         = string.Empty;
        public Uri? URL             = null;
        public string Expression    = string.Empty;
        public DateTime Date        = DateTime.UnixEpoch;
        public string History       = string.Empty;
        public FeedStatus Status    = FeedStatus.INIT;
        public Feed() { }       //null constructor
        public Feed(int PK)     //stub constructor
        {
            ID = PK;
            Status = FeedStatus.STUB;
        }
        public Feed(Feed copy)  //copy constructor
        {
            ID = copy.ID;
            Title = copy.Title;
            URL = copy.URL;
            Expression = copy.Expression;
            Date = copy.Date;
            History = copy.History;
            Status = copy.Status;
        }
        public override string ToString()
        {
            return $"T:{Title}\tU:{URL}\tE:{Expression}\tH:{History}";
        }
        public string ToLongString()
        {
            StringBuilder builder = new();
            const string sep = "----------";
            //
            builder.AppendLine(sep);
            builder.AppendLine("TITLE: " + Title);
            builder.AppendLine("History: " + History);
            builder.AppendLine("Expression: " + Expression);
            builder.AppendLine("URL: " + URL);
            builder.AppendLine("Date: " + Date);
            builder.Append(sep); //don't end with a new line (append, not AppendLine)
            //
            return builder.ToString();
        }
    }
}
