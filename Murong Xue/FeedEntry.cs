using Murong_Xue.DownloadHandling;
using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

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
        INIT = 0, //only used in constructor
        LINKED = 1 << 0, //has a FeedEntry associated with it.
        UPDATED = 1 << 1, //updated during processing
    }
    public record Feed
    {
        public int ID;
        public string Title;
        public Uri? URL;
        public string Expression;
        public DateTime Date;
        public string History;
        public FeedStatus Status;
        public DownloadStatus? dStatus;
        public Feed() //null constructor
        {
            ID = -1;
            Title = string.Empty;
            URL = null;
            Expression = string.Empty;
            Date = DateTime.UnixEpoch;
            History = string.Empty;
            dStatus = null;
        }
        public Feed(Feed copy)
        {
            ID = copy.ID;
            Title = copy.Title;
            URL = copy.URL;
            Expression = copy.Expression;
            Date = copy.Date;
            History = copy.History;
            Status = copy.Status;
            dStatus = copy.dStatus;
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
