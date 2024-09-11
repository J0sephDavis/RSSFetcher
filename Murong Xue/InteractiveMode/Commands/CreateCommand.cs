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

        public override INTERACTIVE_RESPONSE Handle(string[] args)
        {
            throw new NotImplementedException();
        }
    }
}