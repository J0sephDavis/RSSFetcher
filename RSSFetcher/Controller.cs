using RSSFetcher.DownloadHandling;
using RSSFetcher.FeedData;
using RSSFetcher.InteractiveMode;
using RSSFetcher.Logging;
using RSSFetcher.Logging.Output.Modules;
using RSSFetcher.Logging.Reporting;
using RSSFetcher.Summary;

namespace RSSFetcher
{
    /// <summary>
    /// This class orchestrates the initiation and cooperation of the classes
    /// the bundle of states and methods to rule them all, per-se
    /// In no particular order, these are the operations the controller orchestrates
    /// >get Feeds from file
    /// >add feed from process (UI/&c)
    /// >process Feeds
    /// >edit Feeds from UI
    /// >save Feeds
    /// </summary>
    public class Controller : IDisposable
    {
        public const int MAJOR_VERSION = 2;
        public const int MINOR_VERSION = 1;
        public const int PATCH = 0;
        public static readonly string versionString = $"VERSION: {MAJOR_VERSION}.{MINOR_VERSION}.{PATCH}.";
        // ---
        public static readonly string AppRootDirectory = Path.GetDirectoryName(System.AppContext.BaseDirectory) + Path.DirectorySeparatorChar;
        // ---
        private readonly Logger logger = Logger.GetInstance();
        private readonly DownloadHandler downloadHandler;
        private readonly DataFile rssData;
        private readonly FeedManager feedManager = new();
        private readonly Reporter report = Logger.RequestReporter("CONTRL");
        private readonly ArgResult result;
        public Controller()
        {
            logger.AddModule(new LogConsole());
            logger.AddModule(new LogFile(new(AppRootDirectory + "RSS-F.log")));
            // ---
            rssData = new(new(AppRootDirectory + "rss-config.xml"));
            feedManager.AddFeeds(rssData.ReadFeeds());
            // ---
            downloadHandler = new(new(AppRootDirectory + "Downloads" + Path.DirectorySeparatorChar));
            // ---
            result = ArgResult.RUN;
        }
        public Controller(CommandLineArguments args)
        {
            if (args.Result == ArgResult.EDIT)
                logger.AddModule(new InteractiveConsole());
            else
                logger.AddModule(new LogConsole());
            logger.AddModule(new LogFile(args.LogPath));
            // ---
            rssData = new(args.RSSPath);
            feedManager.AddFeeds(rssData.ReadFeeds());
            // ---
            downloadHandler = new(args.DownloadDirectory);
            result = args.Result;
        }
        /// <summary>
        /// runs the main part of the program
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Run()
        {
            report.TraceVal($"result: {result}");
            switch (result)
            {
                case (ArgResult.EDIT):
                    InteractiveEditor editor = new(this);
                    editor.MainLoop();
                    break;
                case (ArgResult.RUN):
                    await DownloadFeeds();
                    UpdateEntries();
                    break;
                case (ArgResult.NONE):
                case (ArgResult.EXIT):
                default:
                    break;
            }
        }

        public void Dispose()
        {
            logger.Dispose(); //TODO https://stackoverflow.com/questions/151051/when-should-i-use-gc-suppressfinalize
        }

        public void SubscribeFeedAddOrRemove(EventHandler method) => feedManager.FeedAddOrRemove += method;
        //-----------Tasks------------------------------------------------------
        public List<Feed> GetFeeds()
        {
            return feedManager.GetFeeds();
        }
        public Feed? GetFeed(int ID)
        {
            return feedManager.GetFeed(ID);
        }
        public async Task DownloadFeeds()
        {
            report.Trace("DownloadFeeds()");
            foreach (var feed in GetFeeds())
            {
                downloadHandler.AddDownload(new DownloadEntryFeed(feed, downloadHandler));
            }
            await downloadHandler.ProcessDownloads();
        }
        public void UpdateEntries()
        {
            rssData.WriteFeeds(GetFeeds());
        }
        //----------------------------------------------------------------------
        /// <summary>
        /// Add a feed to the model
        /// </summary>
        /// <param name="feed"></param>
        /// <returns>the feed to link in the model</returns>
        public bool CreateFeed(Feed feed)
        {
            report.Trace("Create Feed");
            if (feed.URL == null) return false;
            feedManager.AddFeed(feed);
            return true;
        }
        public bool DeleteFeed(Feed? feed)
        {
            if (feed == null)
            {
                report.Warn("Attempting to delete null feed");
                return false;
            }
            report.Trace($"Delete feed {feed}");
            bool IsRemoved = false;
            if ((feed.Status & FeedStatus.LINKED) != 0)
            {
                IsRemoved = feedManager.RemoveFeed(feed.ID);
                if (IsRemoved == false)
                    report.Warn("rss.RemoveFeed - feed not removed.");
                return IsRemoved;
            }
            else report.TraceVal($"RemoveFeed: feed not linked, status={feed.Status}");
            return IsRemoved;
        }
        // ---
        public string GetSummary()
        {
            SummaryBuilder summary = new();
            summary.Add(downloadHandler);
            summary.Add(rssData);
            return summary.ToString();
        }
    }
}