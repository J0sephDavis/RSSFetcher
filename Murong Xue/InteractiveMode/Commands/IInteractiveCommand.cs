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
        protected static string? Prompt(string msg, int minLen = 1)
        {
            Console.Write(msg);
            string? input = Console.ReadLine();
            if (input != null && input.Length < minLen)
                return null;
            return input;
        }
        /// <summary>
        /// provide a command e.g., "quit" or "exit" and
        /// the command will return true if this class can handle it.
        /// Expects command in lowercases
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Whether the input command string is valid for this command</returns>
        abstract public bool CommandMatch(string command);
        //      TODO  abstract public string GetDescription();

        /// <summary>
        /// processes the command with the given arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        abstract public INTERACTIVE_RESPONSE Handle(string[] args);
    }
}
