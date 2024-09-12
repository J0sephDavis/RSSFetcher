using RSSFetcher.FeedData;
using RSSFetcher.Logging.Reporting;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode.Commands
{
    public class EditCommand(Controller control, InteractiveReporter report) : IInteractiveCommand(control, report)
    {
        static readonly string[] valid_commands = ["edit"];
        public override string GetName() => valid_commands[0];
        public override string GetHelp() => "accepts the index of the feed to be edited";
        public override bool CommandMatch(string command)
        {
            foreach (string cmd in valid_commands)
                if (cmd == command)
                    return true;
            return false;
        }
        public override INTERACTIVE_RESPONSE Handle(string[] args)
        {
            if (args.Length < 2) return INTERACTIVE_RESPONSE.FAILURE;
            if (int.TryParse(args[1], out int index) == false) return INTERACTIVE_RESPONSE.FAILURE;
            Feed? feed = controller.GetFeed(index);
            if (feed == null) return INTERACTIVE_RESPONSE.FAILURE;
            // ---
            report.Interactive($"EDITING: {index}");
            report.Interactive(feed.ToLongString());
            //TITLE
            string? input;
            input = Prompt(CreateCommand.field_title);
            if (input != null)
                feed.Title = input;
            // URL
            input = Prompt(CreateCommand.field_url, 8); //magic number,  has no real meaning. I doubt there is an RSS feed shorter than this
            if (input != null)
                Uri.TryCreate(input, UriKind.Absolute, out feed.URL);
            // EXPRESSION
            input = Prompt(CreateCommand.field_expr);
            if (input != null)
                feed.Expression = input;
            // HISTORY
            input = Prompt(CreateCommand.field_history);
            if (input != null)
            {
                feed.History = input;
                feed.Date = DateTime.UnixEpoch;
            }
            report.Interactive($"NEW {feed.ToLongString()}");
            // ---
            return INTERACTIVE_RESPONSE.SUCCESS;
        }
    }
}
