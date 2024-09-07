using RSSFetcher;
using RSSFetcher.FeedData;
using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;
using static Murong_Xue.InteractiveMode.InteractiveEditor;

namespace Murong_Xue.InteractiveMode
{
    abstract class IInteractiveCommand(Controller _controller, InteractiveReporter _report)
    {
        protected readonly Controller controller = _controller;
        protected readonly InteractiveReporter report = _report;
        /// <summary>
        /// resturns the command name, e.g., "print"
        /// </summary>
        /// <returns></returns>
        abstract public string GetName();
        //      TODO  abstract public string GetDescription();

        /// <summary>
        /// processes the command with the given arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        abstract public INTERACTIVE_RESPONSE Handle(string[] args);
    }

    internal class PrintCommand(Controller control, InteractiveReporter report)
        : IInteractiveCommand(control, report)
    {
        public override string GetName() => "print";
        public override INTERACTIVE_RESPONSE Handle(string[] args)
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
            return INTERACTIVE_RESPONSE.SUCCESS;
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
    internal class QuitCommand(Controller control, InteractiveReporter report) : IInteractiveCommand(control, report)
    {
        public override string GetName() => "quit";
        public override INTERACTIVE_RESPONSE Handle(string[] args)
        {
            return INTERACTIVE_RESPONSE.QUIT;
        }
    }
}
