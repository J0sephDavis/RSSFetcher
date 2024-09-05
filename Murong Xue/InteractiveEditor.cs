using Murong_Xue.Logging;
using Murong_Xue.Logging.Reporting;

namespace Murong_Xue
{
    internal class InteractiveEditor
    {
        readonly List<Feed> Feeds;
        readonly Reporter report;

        public InteractiveEditor(List<Feed> Feeds)
        {
            this.Feeds = Feeds;
            report = Logger.RequestReporter("EDITOR");
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
            Feed? feed;
            DateTime _today = DateTime.Now;
            int daysSince;
            if (index != null && index > -1 && index < Feeds.Count)
            {
                feed = Feeds[(int)index];
                if (feed.Date != DateTime.UnixEpoch)
                {
                    daysSince = (_today - feed.Date).Days;
                }
                else daysSince = -1;
                report.Out(feed.ToLongString());
                return;
            }
            int totalFeeds = Feeds.Count;
            report.Out("ID\tDays\tTitle");
            for (int idx = 0; idx < totalFeeds; idx++)
            {
                feed = Feeds[idx];
                if (feed.Date != DateTime.UnixEpoch)
                {
                    daysSince = (_today - feed.Date).Days;
                }
                else daysSince = -1;

                report.Out($"{idx}\t{daysSince}\t{feed.Title}");
            }
        }

        protected void PromptForInput(string prompt, out string? input, uint minLen = 3)
        {
            report.Interactive(prompt);
            input = Console.ReadLine();
            if (input != null && input.Length < minLen)
            {
                report.Warn("input len < minimum or NULL.");
                report.Out("Input discarded");
                input = null;
            }
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
                            catch (System.UriFormatException e)
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
                feed.Title= input;
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
        public bool MainLoop()
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
                INTERACTIVE_OPTIONS op = INTERACTIVE_OPTIONS.NONE;
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
                //-------- SET OP
                switch (input_args[0])
                {
                    case "help":
                        report.Out("print #?/Edit #/Delete #/Create/Save/Exit/Help");
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
