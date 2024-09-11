using RSSFetcher.FeedData;
using RSSFetcher.InteractiveMode.Commands;
using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace RSSFetcher.InteractiveMode
{
    internal class InteractiveEditor
    {
        private readonly List<Feed> Feeds = [];
        private readonly Controller controller;
        private readonly InteractiveReporter report;
        private readonly List<IInteractiveCommand> Commands = [];
        internal InteractiveEditor(Controller _controller)
        {
            controller = _controller;
            // ---
            report = Logger.RequestInteractive("EDITOR");
            report.PauseOutput();
            //----
            Commands.Add(new PrintCommand(controller, report));
            Commands.Add(new QuitCommand(controller, report));
        }
        public enum INTERACTIVE_RESPONSE //INTERACTIVE_STATUS?
        {
            NONE = -1,
            FAILURE = 0,
            SUCCESS = 1,
            QUIT
        }
        protected string[] PromptForInput(string prompt_msg, uint minLen = 3)
        {
            report.PauseOutput();
            // ---
            Console.Write(prompt_msg);
            string? input = Console.ReadLine();
            // ---
            report.UnpauseOutput();
            // ---
            if (input == null || input.Length < minLen)
                return [string.Empty];
            return input.Split(" ");
        }
        public void MainLoop()
        {
            report.Trace("INTERACTIVE MAIN LOOP");
            string[] input_string = ["help"];
            string input_command;
            INTERACTIVE_RESPONSE response = INTERACTIVE_RESPONSE.NONE;
            // ---
            do
            {
                input_command = input_string[0].ToLower();
                // ---
                report.PauseOutput();
                foreach (var cmd in Commands)
                {
                    if (cmd.GetName() == input_command)
                    {
                        response = cmd.Handle(input_string, out string command_response);
                        report.Interactive(command_response);
                        break;
                    }
                }
                report.UnpauseOutput();
                // ---
                input_string = PromptForInput("> ");

            } while (response != INTERACTIVE_RESPONSE.QUIT);
        }
    }
}
