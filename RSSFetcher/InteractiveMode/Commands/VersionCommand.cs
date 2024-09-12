using RSSFetcher.Logging.Reporting;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode.Commands
{
    public class VersionCommand(Controller control, InteractiveReporter report) : IInteractiveCommand(control, report)
    {
        static readonly string[] valid_commands = ["version"];
        public override string GetHelp() => "no args, gets version information";

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
            report.Interactive(Controller.versionString);
            return INTERACTIVE_RESPONSE.SUCCESS;
        }
    }
}