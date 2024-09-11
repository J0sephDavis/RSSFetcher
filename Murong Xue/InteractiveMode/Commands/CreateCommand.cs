using RSSFetcher.FeedData;
using RSSFetcher.Logging.Reporting;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode.Commands
{
    internal class CreateCommand(Controller control, InteractiveReporter report) : IInteractiveCommand(control, report)
    {
        static readonly string[] valid_commands = ["create", "new"];
        public override string GetName() => valid_commands[0];
        public override bool CommandMatch(string command)
        {
            foreach (string cmd in valid_commands)
                if (cmd == command)
                    return true;
            return false;
        }
        public const string field_title =      "Title:  ";
        public const string field_url =        "URL:    ";
        public const string field_expr =       "Regex:  ";
        public const string field_history =    "History:";
        public override INTERACTIVE_RESPONSE Handle(string[] args)
        {
            Feed feed = new();
            report.Interactive("leave fields blank for default values");
            string? input;
            // TITLE
            input = Prompt(field_title);
            if (input != null)
                feed.Title = input;
            else
            {
                report.Error("Title may not be empty");
                return INTERACTIVE_RESPONSE.FAILURE;
            }
            // URL
            input = Prompt(field_url);
            if (input != null)
                Uri.TryCreate(input, UriKind.Absolute, out feed.URL);
            else
            {
                report.Error($"URL may not be null. input->{input}");
                return INTERACTIVE_RESPONSE.FAILURE;
            }
            // EXPR
            input = Prompt(field_expr);
            feed.Expression = input ?? ".*";
            // HISTORY
            input = Prompt(field_history);
            feed.History = input ?? "no-history";
            feed.Date = DateTime.UnixEpoch;
            // ---
            report.Interactive(feed.ToLongString());
            if (controller.CreateFeed(feed))
                return INTERACTIVE_RESPONSE.SUCCESS;
            else
            {
                report.Warn("failed to create feed");
                return INTERACTIVE_RESPONSE.FAILURE;
            }
        }
    }
}