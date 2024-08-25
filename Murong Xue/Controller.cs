using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* ----- Create/Request/Update/Delete
 * wrapper/controller to return view & convert to model
 R* GetEntries() -> List<FEED RECORD>
 U* EditEntry(ID, FLAGS, RECORD)? Maybe make this a method of FEED RECORD
 C* CreateEntry(RECORD) -> ID - returns ID of new entry
 D* Delete Entry(ID) - Deletes the entry
*/
namespace Murong_Xue
{
    /*public record Feed
    {
        public int ID;
        string Title;
        Uri URL;
        string Expression;
        DateTime Date;
        string History;

        public Feed(int id, string title, Uri url, string expression, DateTime Date, string history)
        {
            ID = id;
            Title = title;
            URL = url;
            Expression = expression;
            this.Date = Date;
            History = history;
        }

        public string ToStringEntry()
        {
            return $"{ID} {Title} {Date}";
        }

        public string[] ToStringList()
        {
            return [Title, $"{Date}"];
        }
    }*/
    public class Controller
    {
        private EntryData rss;
        private List<FeedEntry> feeds;
        
        public Controller()
        {
            rss = new EntryData(Config.GetInstance().GetRSSPath());
            feeds = rss.GetFeeds();
            for (int ind = 0; ind < feeds.Count; ind++)
            {
                FeedEntry c_feed = feeds[ind];
                Feed feed = new(c_feed.original);
                Feeds.Add(feed);
            }
        }

        private List<Feed> Feeds = [];
        public List<Feed> GetFeeds()
        {
            return Feeds;
        }
        public Feed? GetFeed(int ID)
        {
            foreach (var feed in Feeds)
                if (feed.ID == ID)
                    return feed;
            return null;
        }
        public int UpdateFeed(Feed feed)
        {
            Feed? currentFeed = GetFeed(feed.ID);
            if (currentFeed == null) return -1;
            currentFeed = feed; //this should update it?
            return feed.ID;
        }
        public int CreateFeed(Feed feed)
        {
            if (GetFeed(feed.ID) != null)
                return -1;
            Feeds.Add(feed);
            return feed.ID;
        }
        public int DeleteFeed(Feed feed)
        {
            Feeds.Remove(feed);
            return feed.ID;
        }
    }
}
