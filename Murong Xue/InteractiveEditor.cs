using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Murong_Xue
{
    internal class InteractiveEditor
    {
        List<FeedData> Feeds;
        public InteractiveEditor(List<FeedData> Feeds)
        {
            this.Feeds = Feeds;
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
                    idx, entry.GetTitle(), entry.GetHistory());
            }
        }
        protected void EditHandler(int? index)
        {
            const string edit_help = @"(T)itle/(H)istory/(E)xpression/(U)rl\E(X)it/(P)RINT";
            if (index == null || index < 0 || index >= Feeds.Count)
            {
                Console.WriteLine("EDIT: Invalid index {0}", index);
                return;
            }
            FeedData entry = Feeds[(int)index];
            PrintHandler(index);
            Console.WriteLine(edit_help);
            //----
            int input_char = 0;
            string _title = entry.GetTitle();
            string _history = entry.GetHistory();
            string _url = entry.GetURL();
            string _expr = entry.GetExpr();
            do
            {
                Console.Write("E>");
                string? input = Console.ReadLine();
                if (input == null)
                    continue;
                input = input.ToLower();
                input_char = input[0];
                Console.Write("EDIT:");
                switch (input_char)
                {
                    case 't':
                        Console.WriteLine("Current Title:\t>{0}", _title);
                        Console.Write("Title:");

                        input = Console.ReadLine();
                        if (input == null || input.Length == 0) break;

                        _title = input;
                        Console.WriteLine("New Title:\t>{0}", _title);
                        break;
                    case 'h':
                        Console.WriteLine("Current History:\t>{0}", _history);
                        Console.Write("History:");

                        input = Console.ReadLine();
                        if (input == null || input.Length == 0) break;

                        _history = input;
                        Console.WriteLine("New History:\t>{0}", _history);
                        break;
                    case 'e':
                        Console.WriteLine("Current Expression:\t>{0}", _expr);
                        Console.Write("Expression:");

                        input = Console.ReadLine();
                        if (input == null || input.Length == 0) break;

                        _expr = input;
                        Console.WriteLine("New Expression:\t>{0}", _expr);
                        break;
                    case 'u':
                        Console.WriteLine("Current URL:\t>{0}", _url);
                        Console.Write("URL:");

                        input = Console.ReadLine();
                        if (input == null || input.Length == 0) break;

                        _url = input;
                        Console.WriteLine("New URL:\t[{0}]", _url);
                        break;
                    //----
                    case 'x':
                        Console.WriteLine("CONFIRM CHANGES: Y/N/back(x)");
                        do
                        {
                            Console.Write("E(X) SAVE?:");
                            input = Console.ReadLine();
                            if (input == null)
                                break;
                            input = input.ToLower();
                            input_char = input[0];
                            switch(input_char)
                            {
                                case 'y':
                                    Console.WriteLine("Changes SAVED");
                                    return;
                                case 'n':
                                    Console.WriteLine("Changes DISCARDED");
                                    return;
                               default:
                                    continue;
                            }

                        } while (input_char != 'z');
                        input_char = 0;
                        break;
                    default:
                        Console.WriteLine(edit_help);
                        break;
                }
            } while (input_char != 'x');
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
            FeedData newEntry = new FeedData(_title, _url, _expr, _history);
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
            string? input = null;
            string[] input_args = [];
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
