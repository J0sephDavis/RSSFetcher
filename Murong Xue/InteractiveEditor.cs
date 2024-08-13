using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal class InteractiveEditor
    {
        readonly List<FeedData> Feeds;
        readonly Reporter report;
        // TODO either edit the base functionality for the logger or inherit the class & make on specifically for the editor
        //The editor should be able to hide all the prefixes for printout (but keep them in the saved logfile)
        //Maybe this is just how FEEDBACK works
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
            if (index != null && index > -1 && index < Feeds.Count)
            {
                entry = Feeds[(int)index];
                Console.WriteLine(
                    "----------\n" +
                    "{0}\tTITLE:{1}\n" +
                    "\tHistory:{2}\n" +
                    "\tExpression:{3}\n" +
                    "\tURL:{4}\n" +
                    "----------",
                    index,
                    entry.GetTitle(),
                    entry.GetHistory(),
                    entry.GetExpr(),
                    entry.GetURL()
                );
                return;
            }
            int totalFeeds = Feeds.Count;
            for (int idx = 0; idx < totalFeeds; idx++)
            {
                entry = Feeds[idx];
                Console.WriteLine("{0}\t{1}",
                    idx, entry.GetTitle());
            }
        }

         protected void PromptForInput(string prompt, out string? input, uint minLen = 3)
        {
            report.Log(LogFlag.DEBUG_SPAM, "Prompting for input");
            Console.Write(prompt);
            input = Console.ReadLine();
            if (input != null && input.Length < minLen){
                report.Log(LogFlag.FEEDBACK | LogFlag.WARN, $"input.len < {minLen} or NULL. DISCARDING");
                input = null;
            }
            report.Log(LogFlag.DEBUG_SPAM, $"[Input:{input}]");
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
                report.Log(LogFlag.FEEDBACK, edit_prefix + $"Invalid index {index}");
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
            
            while(true)
            {
                Console.Write(edit_prefix);
                string? input = Console.ReadLine();
                if (input == null || input == string.Empty)
                {
                    //help cmd
                    report.Log(LogFlag.FEEDBACK, edit_help);
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
                        report.Log(LogFlag.FEEDBACK, "Set title back to original");
                        _title = entry.GetTitle();
                        if ((edits & EditFlag.TITLE) != EditFlag.NONE)
                            edits -= EditFlag.TITLE;
                        report.Log(LogFlag.DEBUG, $"Post-EditFlag: {edits}");
                        continue;
                    }
                    edits |= EditFlag.TITLE;
                    _title = input;

                    report.Log(LogFlag.NOTEWORTHY, $"New Title: {_title}");
                    continue;
                }
                if (edit_cmds_history.Contains(input))
                {
                    string history_prompt =
                        $"Current History:\t[{_history}]\n" +
                        $"{edit_prefix}History:";

                    PromptForInput(history_prompt, out input);
                    if (input == null)
                    {
                        report.Log(LogFlag.FEEDBACK, "Set history to original");
                        _title = entry.GetTitle();
                        if ((edits & EditFlag.HISTORY) != EditFlag.NONE)
                            edits -= EditFlag.HISTORY;
                        report.Log(LogFlag.DEBUG, $"Post-EditFlag: {edits}");
                        continue;
                    }
                    edits |= EditFlag.HISTORY;
                    _history = input;

                    report.Log(LogFlag.NOTEWORTHY, $"New History: {_history}");
                    continue;
                }
                if (edit_cmds_expr.Contains(input))
                {
                    string expression_prompt =
                        $"Current Expression:\t[{_expr}]\n" +
                        $"{edit_prefix}Regex:";

                    PromptForInput(expression_prompt, out input);
                    if (input == null)
                    {
                        report.Log(LogFlag.FEEDBACK, "Set expression to original");
                        _title = entry.GetTitle();
                        if ((edits & EditFlag.EXPR) != EditFlag.NONE)
                            edits -= EditFlag.EXPR;
                        report.Log(LogFlag.DEBUG, $"Post-EditFlag: {edits}");
                        continue;
                    }
                    edits |= EditFlag.EXPR;
                    _expr = input;

                    report.Log(LogFlag.NOTEWORTHY, $"New Regex: {_expr}");
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
                        report.Log(LogFlag.FEEDBACK, "Set URL to original");
                        _title = entry.GetTitle();
                        if ((edits & EditFlag.URL) != EditFlag.NONE)
                            edits -= EditFlag.URL;
                        report.Log(LogFlag.DEBUG, $"Post-EditFlag: {edits}");
                        continue;
                    }
                    edits |= EditFlag.URL;
                    _url = input;

                    report.Log(LogFlag.NOTEWORTHY, $"New URL: {_url}");
                    continue;
                }
                if (edit_cmds_conf.Contains(input))
                {
                    report.Log(LogFlag.FEEDBACK,"Save Changes? y/n/back(any other input)" + $"\tChanges:{edits}");
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
                            report.Log(LogFlag.FEEDBACK, "Entry saved");

                            if ((edits & EditFlag.TITLE) > 0)
                                entry.SetTitle(_title);
                            if ((edits & EditFlag.HISTORY) > 0)
                                entry.SetHistory(_history);
                            if ((edits & EditFlag.EXPR) > 0)
                                entry.SetExpr(_expr);
                            if ((edits & EditFlag.URL) > 0)
                                entry.SetURL(_url);

                            Console.WriteLine("SAVED (WIP)");
                            
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
                        report.Log(LogFlag.FEEDBACK, response);
                    }
                    continue;
                }
                //help cmd
                report.Log(LogFlag.FEEDBACK,edit_help);
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
            while(_title == string.Empty)
            {
                Console.Write("Title:");
                input = Console.ReadLine();
                if (input == null) continue;
                _title = input;
            }
            while(_url == string.Empty)
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
            
            while(_expr == string.Empty)
            {
                Console.Write("Expression:");
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
            FeedData newEntry = new(_title, _url, _expr, _history);
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
            Console.WriteLine("\n!!!---INTERACTIVE MODE---!!!");
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
