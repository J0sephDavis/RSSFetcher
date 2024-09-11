using RSSFetcher.Logging.Reporting;
using System.Diagnostics;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode.Commands
{
    internal class SaveCommand(Controller control, InteractiveReporter report) : IInteractiveCommand(control, report)
    {
        static readonly string[] valid_commands = ["save", "write"];
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
            //todo allow saving to different files / saving subsets to files
            controller.UpdateEntries();
            return INTERACTIVE_RESPONSE.SUCCESS;
        }
    }
}