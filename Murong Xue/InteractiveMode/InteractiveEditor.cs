using RSSFetcher;
using RSSFetcher.FeedData;
using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace Murong_Xue.InteractiveMode
{

    internal class InteractiveEditor
    {
        private readonly List<Feed> Feeds = [];
        private readonly Controller controller = new();
        private readonly Reporter report = Logger.RequestReporter("EDITOR");
        private readonly List<IInteractiveCommand> Commands = [];
        public InteractiveEditor()
        {
            Logger.SetInteractiveMode(true);
            report = Logger.RequestReporter("EDITOR");
        }
        enum INTERACTIVE_OPTION
        {
            NONE,
            EDIT,
            DELETE,
            PRINT,
            CREATE,
            LOAD,
            SAVE,
            EXIT
        };
        /// <summary>
        /// converts single word into INTERACTIVE_OPTION
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>the INTERCTIVE_OPTION associated with the word (NONE by default)</returns>
        private static INTERACTIVE_OPTION CommandToOption(string? cmd)
        {
            if (cmd == null) return INTERACTIVE_OPTION.NONE;
            switch (cmd.ToLower())
            {
                case "edit":
                    return INTERACTIVE_OPTION.EDIT;
                case "delete":
                    return INTERACTIVE_OPTION.DELETE;
                case "print":
                    return INTERACTIVE_OPTION.PRINT;
                case "create":
                    return INTERACTIVE_OPTION.PRINT;
                case "load":
                    return INTERACTIVE_OPTION.LOAD;
                case "save":
                    return INTERACTIVE_OPTION.SAVE;
                case "exit":
                case "quit":
                    return INTERACTIVE_OPTION.EXIT;
                default:
                    return INTERACTIVE_OPTION.NONE;
            }
        }
        protected string[] PromptForInput(string prompt_msg, uint minLen = 3)
        {
            report.Interactive(prompt_msg); //TODO make a dedicate method in Logger that will delay output until we've set a flag/gotten the user input

            string? input = Console.ReadLine();
            report.DebugVal($"PromptForInput, input={input}");
            if (input == null)
                return [string.Empty];
            return input.Split(" ");
        }
        public void MainLoop()
        {
            /* 1. Prompt user for input
             * 2. process command and pass arguments to handler
             */
            report.Trace("MAIN LOOP 2!");
            string[] input_string = [];
            INTERACTIVE_OPTION command = INTERACTIVE_OPTION.NONE;
            while (true)
            {
                //1. prompt for input & return arr<string>[]
                input_string = PromptForInput(">");
                report.DebugVal("input_string: ");
                foreach (string s in input_string)
                    report.DebugVal($"\t{s}");
                //2. get cmd from input[0]
                command = CommandToOption(input_string[0]);
                //3. pass arguments to command (if != NONE)
                switch (command)
                {
                    default:
                        report.Warn($"{command} is not yet implemented");
                        break;
                }
                break;
            }
        }
        #region Static command strings
        static readonly string[] edit_cmds_title = ["title", "t"];
        static readonly string[] edit_cmds_history = ["history", "h"];
        static readonly string[] edit_cmds_expr = ["expr", "regex", "e", "expression"];
        static readonly string[] edit_cmds_url = ["url", "u"];
        static readonly string[] edit_cmds_conf = ["confirm", "conf", "save", "quit", "x"];
        static readonly string[] edit_cmds_print = ["print", "p"];
        #endregion
        [Flags]
        enum EditFlag
        {
            NONE = 0,
            TITLE = 1 << 0,
            HISTORY = 1 << 1,
            URL = 1 << 2,
            EXPR = 1 << 3,
        };

        static readonly string edit_help = @"Title / History / Expr / Url / Confirm (X) / Help (default) / Print";
        protected void EditHandler(int? index)
        {

            if (index == null || index < 0 || index >= Feeds.Count)
            {
                report.Log(LogType.OUTPUT | LogType.ERROR, LogMod.NORMAL, "Index out of bounds"); //output + warn
                return;
            }
            Feed entry = Feeds[(int)index];
            PrintHandler(index);
            report.Out(edit_help);
            //----
            EditFlag edits = EditFlag.NONE;
            string _title = entry.Title;
            string _history = entry.History;
            string _url = entry.URL.ToString();
            string _expr = entry.Expression;

            while (true)
            {
                report.Interactive("");
                string? input = Console.ReadLine();
                if (input == null || input == string.Empty)
                {
                    //help cmd
                    report.Out(edit_help);
                    continue;
                }
                else
                    input = input.ToLower();
                //---
                if (edit_cmds_title.Contains(input))
                {
                    const string title_prompt = "Title:";
                    report.Out($"Current Title:\t[{_title}]");
                    PromptForInput(title_prompt, out input);
                    if (input == null)
                    {
                        report.Out("Reset title back to original");
                        _title = entry.Title;
                        if ((edits & EditFlag.TITLE) != EditFlag.NONE)
                            edits -= EditFlag.TITLE;
                        continue;
                    }
                    edits |= EditFlag.TITLE;
                    _title = input;

                    report.Out($"New Title: {_title}");
                    continue;
                }
                if (edit_cmds_history.Contains(input))
                {
                    const string history_prompt = "History:";
                    report.Out($"Current History:\t[{_history}]");

                    PromptForInput(history_prompt, out input, 0);
                    if (input == null)
                    {
                        report.Out("Set history to original");
                        _title = entry.Title;
                        if ((edits & EditFlag.HISTORY) != EditFlag.NONE)
                            edits -= EditFlag.HISTORY;
                        continue;
                    }
                    edits |= EditFlag.HISTORY;
                    _history = input;

                    report.Out($"New History: {_history}");
                    continue;
                }
                if (edit_cmds_expr.Contains(input))
                {
                    const string expression_prompt = "Regex:";
                    report.Out($"Current Expression:\t[{_expr}]");

                    PromptForInput(expression_prompt, out input, 0);
                    if (input == null)
                    {
                        report.Out("Set expression to original");
                        _title = entry.Title;
                        if ((edits & EditFlag.EXPR) != EditFlag.NONE)
                            edits -= EditFlag.EXPR;
                        continue;
                    }
                    edits |= EditFlag.EXPR;
                    _expr = input;

                    report.Out($"New Regex: {_expr}");
                    continue;
                }
                if (edit_cmds_url.Contains(input))
                {
                    const string url_prompt = "URL:";
                    report.Out($"Current URL:\t[{_url}]");

                    PromptForInput(url_prompt, out input);
                    if (input == null)
                    {
                        report.Out("Set URL to original");
                        _title = entry.Title;
                        if ((edits & EditFlag.URL) != EditFlag.NONE)
                            edits -= EditFlag.URL;
                        continue;
                    }
                    edits |= EditFlag.URL;
                    _url = input;

                    report.Out($"New URL: {_url}");
                    continue;
                }
                if (edit_cmds_conf.Contains(input))
                {
                    report.Out($"Changes made to fields: {edits}");
                    report.Interactive("Save? y/n Back(any)");
                    //---
                    input = Console.ReadLine();
                    if (input == null || input == string.Empty)
                        input = " ";
                    else
                        input = input.ToLower();
                    //---
                    switch (input[0])
                    {
                        case 'y':
                            if ((edits & EditFlag.TITLE) > 0)
                                entry.Title = _title;
                            if ((edits & EditFlag.HISTORY) > 0)
                                entry.History = _history;
                            if ((edits & EditFlag.EXPR) > 0)
                                entry.Expression = _expr;
                            try
                            {
                                if ((edits & EditFlag.URL) > 0)
                                    entry.URL = new(_url);
                            }
                            catch (UriFormatException e)
                            {
                                report.Error($"{e.Message}");
                                report.Warn("Change to URL DISCARDED");
                            }
                            report.Out("SAVED");
                            return;
                        case 'n':
                            report.Out("DISCARDED");
                            return;
                        default:
                            report.Out("BACK");
                            break;
                    }
                    continue;
                }
                if (edit_cmds_print.Contains(input))
                {
                    PrintHandler(index);
                    if (edits != EditFlag.NONE)
                    {
                        string response = $"{edits}";

                        if ((edits & EditFlag.TITLE) > 0)
                            response += $"\n_Title:{_title}";
                        if ((edits & EditFlag.HISTORY) > 0)
                            response += $"\n_History:{_history}";
                        if ((edits & EditFlag.EXPR) > 0)
                            response += $"\n_Expr:{_expr}";
                        if ((edits & EditFlag.URL) > 0)
                            response += $"\n_URL:{_url}";
                        //----
                        report.Out(response);
                    }
                    continue;
                }
                //help cmd
                report.Out(edit_help);
            }
        }
        protected void DeleteHandler(int? index)
        {
            if (index == null || index < 0 || index > Feeds.Count)
            {
                report.Out($"DELETE: Invalid index {index}");
                return;
            }
            //----
            PrintHandler(index);
            //----
            int input_char = 0;
            string? input;
            do
            {
                report.Interactive("DELETE? (Y/N): ");
                //-----
                input = Console.ReadLine();
                if (input == null || input == string.Empty)
                    continue;
                input = input.ToLower();
                input_char = input[0];
                //-----
                switch (input_char)
                {
                    case 'y':
                        report.Out("DELETED");
                        Feeds.Remove(Feeds[(int)index]);
                        return;
                    case 'n':
                        report.Out("CANCELLED");
                        return;
                    default:
                        continue;
                }
            } while (input_char != 'z');
        }
        protected void CreateHandler()
        {
            Feed feed = new();

            string? input;
            while (feed.Title == string.Empty)
            {
                report.Out("Title:");
                input = Console.ReadLine();
                if (input == null) continue;
                feed.Title = input;
            }
            while (feed.URL.ToString() == string.Empty)
            {
                report.Out("URL:");
                input = Console.ReadLine();
                if (input == null) continue;
                feed.URL = new(input); //exception point
            }
            report.Out("History(can be null):");
            input = Console.ReadLine();
            if (input != null && input.Length > 1)
                feed.History = input;

            while (feed.Expression == string.Empty)
            {
                report.Out("Expression:");
                input = Console.ReadLine();
                if (input == null) continue;
                feed.Expression = input;
            }

            report.Out("NEW ENTRY:\n" +
                $"\tTitle:\t\t{feed.Title}\n" +
                $"\tHistory:\t{feed.History}\n" +
                $"\tExpression:\t{feed.Expression}\n" +
                $"\tURL:\t\t{feed.URL}"
            );
            feed.ID = Feeds.Count;
            Feeds.Add(new(feed));
        }
        public bool MainLoop_old()
        {
            int totalFeeds = Feeds.Count;
            if (totalFeeds == 0)
            {
                report.Error("No Entries found");
                return false;
            }

            report.Debug("\n!!!---INTERACTIVE MODE---!!!");
            Logger.SetInteractiveMode(true);
            string? input;
            string[] input_args;
            int value = -1;
            bool parsedInt;
            while (true)
            {
                INTERACTIVE_OPTION op = INTERACTIVE_OPTION.NONE;
                report.Interactive("");
                input = Console.ReadLine();
                if (input != null)
                    input_args = input.Split(" ");
                else
                    input_args = [];
                //---
                if (input_args.Length > 1)
                    parsedInt = int.TryParse(input_args[1], out value);
                else parsedInt = false;
                if (!parsedInt)
                    value = -1;
                //---------- HANDLE OPS
                switch (op)
                {
                    case INTERACTIVE_OPTION.PRINT:
                        PrintHandler(value);
                        break;
                    case INTERACTIVE_OPTION.EDIT:
                        EditHandler(value);
                        break;
                    case INTERACTIVE_OPTION.DELETE:
                        DeleteHandler(value);
                        break;
                    case INTERACTIVE_OPTION.CREATE:
                        CreateHandler();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
