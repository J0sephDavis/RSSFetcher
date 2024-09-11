﻿using RSSFetcher.Logging.Output.Modules;
using RSSFetcher;
using RSSFetcher.FeedData;
using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;
using System.Security.Cryptography;

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
        public enum INTERACTIVE_RESPONSE
        {
            NONE = -1,
            FAILURE = 0,
            SUCCESS = 1,
            QUIT
        }
        protected string[] PromptForInput(string prompt_msg, uint minLen = 3)
        {
            report.PauseOutput();
            Console.Write(prompt_msg);
            string? input = Console.ReadLine();
            report.UnpauseOutput();
            if (input == null)
                return [string.Empty];
            return input.Split(" ");
        }
        public void MainLoop()
        {
            report.Trace("INTERACTIVE MAIN LOOP");
            string[] input_string = ["help"];
            while (true)
            {
                string command_string = input_string[0].ToLower();
                INTERACTIVE_RESPONSE response = INTERACTIVE_RESPONSE.NONE;
                foreach (var cmd in Commands)
                {
                    if (cmd.GetName() == command_string)
                    {
                        response = cmd.Handle(input_string);
                        break;
                    }
                }
                report.Trace("AFTER FOREACH");
                if (response == INTERACTIVE_RESPONSE.QUIT) return;
                // ---
                input_string = PromptForInput("> ");
            }
        }
    }
}
