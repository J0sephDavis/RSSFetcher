using RSSFetcher.Logging.Reporting;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode.Commands
{
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
