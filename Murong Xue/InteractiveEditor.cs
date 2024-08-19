namespace Murong_Xue
{
    internal class InteractiveEditor
    {
        readonly List<FeedData> Feeds;
        readonly Reporter report;

        public InteractiveEditor(List<FeedData> Feeds)
        {
            this.Feeds = Feeds;
            report = Config.OneReporterPlease("Editor");
        }
        enum INTERACTIVE_OPTIONS
        {
            NONE,
            EDIT,
            DELETE,
            PRINT,
            CREATE,
        };
        protected void PrintHandler(int? index)
        {
            FeedData? entry;
            DateTime _today = DateTime.Now;
            DateTime _date;
            int daysSince;
            if (index != null && index > -1 && index < Feeds.Count)
            {
                entry = Feeds[(int)index];
                if (DateTime.TryParse(entry.GetDate(), out _date))
                {
                    daysSince = (_today - _date).Days;
                }
                else daysSince = -1;

                Console.WriteLine(
                    "----------\n" +
                    "{0}\tTITLE:{1}\n" +
                    "\tHistory:{2}\n" +
                    "\tExpression:{3}\n" +
                    "\tURL:{4}\n" +
                    "\tDays Since:{5}\n" +
                    "----------",
                    index,
                    entry.GetTitle(),
                    entry.GetHistory(),
                    entry.GetExpr(),
                    entry.GetURL(),
                    daysSince
                );
                return;
            }
            int totalFeeds = Feeds.Count;
            Console.WriteLine("ID\tDays\tTitle");
            for (int idx = 0; idx < totalFeeds; idx++)
            {
                entry = Feeds[idx];
                if (DateTime.TryParse(entry.GetDate(), out _date))
                {
                    daysSince = (_today - _date).Days;
                }
                else daysSince = -1;

                Console.WriteLine($"{idx}\t{daysSince}\t{entry.GetTitle()}");
            }
        }

        protected void PromptForInput(string prompt, out string? input, uint minLen = 3)
        {
            report.Trace("Prompting for input");
            Console.Write(prompt);
            input = Console.ReadLine();
            if (input != null && input.Length < minLen)
            {
                report.Warn("input len < minimum or NULL.");
                report.Out("Input discarded");
                input = null;
            }
            report.TraceVal($"[Input:{input}]");
        }

        static readonly string[] edit_cmds_title = ["title", "t"];
        static readonly string[] edit_cmds_history = ["history", "h"];
        static readonly string[] edit_cmds_expr = ["expr", "regex", "e", "expression"];
        static readonly string[] edit_cmds_url = ["url", "u"];
        static readonly string[] edit_cmds_conf = ["confirm", "conf", "save", "quit", "x"];
        static readonly string[] edit_cmds_print = ["print", "p"];
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
        static readonly string edit_prefix = @"E: ";
        protected void EditHandler(int? index)
        {

            if (index == null || index < 0 || index >= Feeds.Count)
            {
                report.Log(LogType.OUTPUT | LogType.ERROR, LogMod.NORMAL, "Index out of bounds"); //output + warn
                return;
            }
            FeedData entry = Feeds[(int)index];
            PrintHandler(index);
            Console.WriteLine(edit_help);
            //----
            EditFlag edits = EditFlag.NONE;
            string _title = entry.GetTitle();
            string _history = entry.GetHistory();
            string _url = entry.GetURL();
            string _expr = entry.GetExpr();

            while (true)
            {
                Console.Write(edit_prefix);
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
                    string title_prompt =
                        $"Current Title:\t[{_title}]\n" +
                        $"{edit_prefix}Title:";

                    PromptForInput(title_prompt, out input);
                    if (input == null)
                    {
                        report.Out("Reset title back to original");
                        _title = entry.GetTitle();
                        if ((edits & EditFlag.TITLE) != EditFlag.NONE)
                            edits -= EditFlag.TITLE;
                        report.TraceVal($"Post-EditFlag: {edits}");
                        continue;
                    }
                    edits |= EditFlag.TITLE;
                    _title = input;

                    report.Out($"New Title: {_title}");
                    continue;
                }
                if (edit_cmds_history.Contains(input))
                {
                    string history_prompt =
                        $"Current History:\t[{_history}]\n" +
                        $"{edit_prefix}History:";

                    PromptForInput(history_prompt, out input, 0);
                    if (input == null)
                    {
                        report.Out("Set history to original");
                        _title = entry.GetTitle();
                        if ((edits & EditFlag.HISTORY) != EditFlag.NONE)
                            edits -= EditFlag.HISTORY;
                        report.TraceVal($"Post-EditFlag: {edits}");
                        continue;
                    }
                    edits |= EditFlag.HISTORY;
                    _history = input;

                    report.Out($"New History: {_history}");
                    continue;
                }
                if (edit_cmds_expr.Contains(input))
                {
                    string expression_prompt =
                        $"Current Expression:\t[{_expr}]\n" +
                        $"{edit_prefix}Regex:";

                    PromptForInput(expression_prompt, out input, 0);
                    if (input == null)
                    {
                        report.Out("Set expression to original");
                        _title = entry.GetTitle();
                        if ((edits & EditFlag.EXPR) != EditFlag.NONE)
                            edits -= EditFlag.EXPR;
                        report.TraceVal($"Post-EditFlag: {edits}");
                        continue;
                    }
                    edits |= EditFlag.EXPR;
                    _expr = input;

                    report.Out($"New Regex: {_expr}");
                    continue;
                }
                if (edit_cmds_url.Contains(input))
                {
                    string url_prompt =
                        $"Current URL:\t[{_url}]\n" +
                        $"{edit_prefix}URL:";

                    PromptForInput(url_prompt, out input);
                    if (input == null)
                    {
                        report.Out("Set URL to original");
                        _title = entry.GetTitle();
                        if ((edits & EditFlag.URL) != EditFlag.NONE)
                            edits -= EditFlag.URL;
                        report.TraceVal($"Post-EditFlag: {edits}");
                        continue;
                    }
                    edits |= EditFlag.URL;
                    _url = input;

                    report.Out($"New URL: {_url}");
                    continue;
                }
                if (edit_cmds_conf.Contains(input))
                {
                    report.Out("Save Changes? y/n/back(any other input)" + $"\tChanges:{edits}");
                    Console.Write(edit_prefix + "?:");
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
                            report.Out("Entry saved");

                            if ((edits & EditFlag.TITLE) > 0)
                                entry.SetTitle(_title);
                            if ((edits & EditFlag.HISTORY) > 0)
                                entry.SetHistory(_history);
                            if ((edits & EditFlag.EXPR) > 0)
                                entry.SetExpr(_expr);
                            try
                            {
                                if ((edits & EditFlag.URL) > 0)
                                    entry.SetURL(_url);
                            }
                            catch (System.UriFormatException e)
                            {
                                report.Error($"{e.Message}");
                                report.Warn("Change to URL DISCARDED");
                            }
                            report.Out("SAVED");
                            return;
                        case 'n':
                            Console.WriteLine("DISCARDED");
                            return;
                        default:
                            Console.WriteLine("BACK");
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
                Console.WriteLine("DELETE: Invalid index {0}", index);
                return;
            }
            //----
            PrintHandler(index);
            //----
            int input_char = 0;
            string? input;
            do
            {
                Console.Write("D> DELETE? (Y/N):");
                //-----
                input = Console.ReadLine();
                if (input == null)
                    continue;
                input = input.ToLower();
                input_char = input[0];
                //-----
                switch (input_char)
                {
                    case 'y':
                        Console.WriteLine("DELETED");
                        Feeds.Remove(Feeds[(int)index]);
                        return;
                    case 'n':
                        Console.WriteLine("CANCELLED");
                        return;
                    default:
                        continue;
                }
            } while (input_char != 'z');
        }
        protected void CreateHandler()
        {
            string _title = string.Empty;
            string _url = string.Empty;
            string _history = "no-history";
            string _expr = string.Empty;

            string? input;
            while (_title == string.Empty)
            {
                Console.Write("Title:");
                input = Console.ReadLine();
                if (input == null) continue;
                _title = input;
            }
            while (_url == string.Empty)
            {
                Console.Write("URL:");
                input = Console.ReadLine();
                if (input == null) continue;
                _url = input;
            }
            Console.Write("History(can be null):");
            input = Console.ReadLine();
            if (input != null && input.Length > 1)
                _history = input;

            while (_expr == string.Empty)
            {
                report.Out("Expression:");
                input = Console.ReadLine();
                if (input == null) continue;
                _expr = input;
            }

            Console.WriteLine("NEW ENTRY:\n" +
                $"\tTitle:\t\t{_title}\n" +
                $"\tHistory:\t{_history}\n" +
                $"\tExpression:\t{_expr}\n" +
                $"\tURL:\t\t{_url}"
            );
            FeedData newEntry = new(_title, _url, _expr, _history, DateTime.UnixEpoch.ToString());
            Feeds.Add(newEntry);
        }
        public bool MainLoop()
        {
            int totalFeeds = Feeds.Count;
            if (totalFeeds == 0)
            {
                Console.WriteLine("no feeds!");
                return false;
            }
            
            report.Debug("\n!!!---INTERACTIVE MODE---!!!");
            Config.EnableInteractiveMode();
            string? input;
            string[] input_args;
            int value = -1;
            bool parsedInt;
            while (true)
            {
                INTERACTIVE_OPTIONS op = INTERACTIVE_OPTIONS.NONE;
                Console.Write("!");
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
                //-------- SET OP
                switch (input_args[0])
                {
                    case "help":
                        Console.WriteLine("print #?/Edit #/Delete #/Create/Save/Exit/Help");
                        break;
                    case "print":
                        op = INTERACTIVE_OPTIONS.PRINT;
                        break;
                    case "delete":
                        op = INTERACTIVE_OPTIONS.DELETE;
                        break;
                    case "edit":
                        op = INTERACTIVE_OPTIONS.EDIT;
                        break;
                    case "create":
                        op = INTERACTIVE_OPTIONS.CREATE;
                        break;
                    case "exit":
                        return false;
                    case "save":
                        return true;
                    default:
                        op = INTERACTIVE_OPTIONS.NONE;
                        break;
                }
                //---------- HANDLE OPS
                switch (op)
                {
                    case INTERACTIVE_OPTIONS.PRINT:
                        PrintHandler(value);
                        break;
                    case INTERACTIVE_OPTIONS.EDIT:
                        EditHandler(value);
                        break;
                    case INTERACTIVE_OPTIONS.DELETE:
                        DeleteHandler(value);
                        break;
                    case INTERACTIVE_OPTIONS.CREATE:
                        CreateHandler();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
