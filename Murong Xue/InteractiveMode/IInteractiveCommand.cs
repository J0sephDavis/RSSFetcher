using RSSFetcher.FeedData;
using RSSFetcher.Logging.Reporting;
using System.Text;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode
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
        abstract public INTERACTIVE_RESPONSE Handle(string[] args, out string response);
    }

    internal class PrintCommand(Controller control, InteractiveReporter report)
        : IInteractiveCommand(control, report)
    {
        public override string GetName() => "print";
        public override INTERACTIVE_RESPONSE Handle(string[] args, out string response)
        {
            const string MsgHeader = "ID\tDays\tTitle";
            StringBuilder responseBuilder = new(MsgHeader);

            if (args.Length == 1)
            {
                foreach (var feed in controller.GetFeeds())
                {
                    int days = (DateTime.Now - feed.Date).Days;
                    responseBuilder.AppendLine($"{feed.ID}\t{days}\t{feed.Title}");
                }
            }
            else
            {
                Feed? feed;
                for (int i = 1; i < args.Length; i++)
                {
                    feed = (int.TryParse(args[i], out int idx)) ? controller.GetFeed(idx) : null;
                    if (feed != null)
                    {
                        int days = (DateTime.Now - feed.Date).Days;
                        responseBuilder.AppendLine($"{feed.ID}\t{days}\t{feed.Title}");
                    }
                }
            }
            //report.Interactive(responseBuilder.ToString());
            response = responseBuilder.ToString();
            return INTERACTIVE_RESPONSE.SUCCESS;
        }
    }
    internal class QuitCommand(Controller control, InteractiveReporter report) : IInteractiveCommand(control, report)
    {
        public override string GetName() => "quit";
        public override INTERACTIVE_RESPONSE Handle(string[] args, out string response)
        {
            response = "OK";
            return INTERACTIVE_RESPONSE.QUIT;
        }
    }
}
