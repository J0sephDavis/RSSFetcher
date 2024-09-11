using RSSFetcher.FeedData;
using RSSFetcher.Logging.Reporting;
using System.Text;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode.Commands
{
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
                    feed = int.TryParse(args[i], out int idx) ? controller.GetFeed(idx) : null;
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
}
