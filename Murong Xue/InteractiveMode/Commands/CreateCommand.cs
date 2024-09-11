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
            throw new NotImplementedException();
        }
    }
}