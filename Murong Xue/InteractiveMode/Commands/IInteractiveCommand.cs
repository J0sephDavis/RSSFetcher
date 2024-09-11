using RSSFetcher.Logging.Reporting;
using static RSSFetcher.InteractiveMode.InteractiveEditor;

namespace RSSFetcher.InteractiveMode.Commands
{
    abstract class IInteractiveCommand(Controller _controller, InteractiveReporter _report)
    {
        protected readonly Controller controller = _controller;
        protected readonly InteractiveReporter report = _report;
        /// <summary>
        /// resturns the command name, e.g., "print"
        /// </summary>
        /// <returns></returns>
        abstract public string GetName();
        //      TODO  abstract public string GetDescription();

        /// <summary>
        /// processes the command with the given arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        abstract public INTERACTIVE_RESPONSE Handle(string[] args, out string response);
    }
}
