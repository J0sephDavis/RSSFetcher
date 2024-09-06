using RSSFetcher;
using RSSFetcher.FeedData;
using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace Murong_Xue.InteractiveMode
{
    abstract class IInteractiveCommand(Controller _controller, Reporter _report)
    {
        protected readonly Controller controller = _controller;
        protected readonly Reporter report = _report;
        protected enum RESPONSE
        {
            SUCCESS = 1,
            FAILURE = 0
        }
        /// <summary>
        /// resturns the command name, e.g., "print"
        /// </summary>
        /// <returns></returns>
        abstract public string GetName();
        /// <summary>
        /// processes the command with the given arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        abstract public int Handle(string[] args);
        abstract public int Handle(int arg);
    }

    internal class PrintCommand(Controller control)
        : IInteractiveCommand(control, Logger.RequestReporter("PRINT"))
    {
        public override string GetName() => "print";
        public override int Handle(int arg)
        {
            Feed? feed = controller.GetFeed(arg);
            if (feed == null) return (int)RESPONSE.FAILURE;
            PrintHeader();
            PrintFeed(feed);
            return (int)RESPONSE.SUCCESS;
        }
        public override int Handle(string[] args)
        {
            PrintHeader();
            if (args.Length == 0)
            {
                foreach (var feed in controller.GetFeeds())
                    PrintFeed(feed);
            }
            else
            {
                Feed? feed;
                foreach (string arg in args)
                {
                    feed = (int.TryParse(arg, out int idx)) ? controller.GetFeed(idx) : null;
                    if (feed != null)
                        PrintFeed(feed);
                }
            }
            return (int)RESPONSE.SUCCESS;
        }
        private void PrintHeader()
        {
            report.Out("ID\tDays\tTitle");
        }
        private void PrintFeed(Feed feed)
        {
            int days = (DateTime.Now - feed.Date).Days;
            report.Out($"{feed.ID}\t{days}\t{feed.Title}");
        }
    }
}
