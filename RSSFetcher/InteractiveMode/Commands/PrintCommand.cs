using RSSFetcher.FeedData;
using RSSFetcher.Logging.Reporting;
using System.Text;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode.Commands
{
    public class PrintCommand(Controller control, InteractiveReporter report)
        : IInteractiveCommand(control, report)
    {
        static readonly string[] valid_commands = ["print"];
        public override string GetHelp() => "no args prints all feeds. or provide indexes separated by a space to only print those feeds";
        public override bool CommandMatch(string command)
        {
            foreach (string cmd in valid_commands)
                if (cmd == command)
                    return true;
            return false;
        }
        public override string GetName() => valid_commands[0];
        public override INTERACTIVE_RESPONSE Handle(string[] args)
        {
            const string MsgHeader = "ID\tDays\tTitle\n";
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
            report.Interactive(responseBuilder.ToString());
            return INTERACTIVE_RESPONSE.SUCCESS;
        }
    }
}
