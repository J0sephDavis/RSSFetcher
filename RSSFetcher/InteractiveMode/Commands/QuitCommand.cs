using RSSFetcher.Logging.Reporting;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode.Commands
{
    public class QuitCommand(Controller control, InteractiveReporter report) : IInteractiveCommand(control, report)
    {
        static readonly string[] valid_commands = ["quit", "exit"];
        public override string GetHelp() => "quits the program";
        public override string GetName() => valid_commands[0];
        public override bool CommandMatch(string command)
        {
            foreach (string cmd in valid_commands)
                if (cmd == command)
                    return true;
            return false;
        }
        public override INTERACTIVE_RESPONSE Handle(string[] args)
        {
            report.Interactive("Quitting");
            return INTERACTIVE_RESPONSE.QUIT;
        }
    }
}
