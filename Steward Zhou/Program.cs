using RSSFetcher;
using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;
namespace Steward_Zhou
{
    internal static class Program
    {
        const int MAJOR_VERSION = 1;
        const int MINOR_VERSION = 0;
        const int PATCH = 0;
        static readonly string VERSION = $"Version: {MAJOR_VERSION}.{MINOR_VERSION}.{PATCH}";
        /*------------------------------------------------------------------------------
        - VIEW - Form will handle the presentation & and conveying input to..
        - CONTROLLER - Basically just MX, but we will edit it to fit more in line with this identity.
        - MODEL - the xml files we are editing, I think.
        ------------------------------------------------------------------------------*/
        static Reporter report;
        [STAThread]
        static void Main()
        {
            report ??= Logger.RequestReporter("W-PROG");
            report.Notice(VERSION);
            Logger.SetLogLevel(new(LogType.ALL, LogMod.ALL));
            //
            ApplicationConfiguration.Initialize();
            MainWindow mainWin = new();
            Application.Run(mainWin);
            report.Out("END OF EXECUTION");
        }
        /*----------------------------------------------|-------------------------------
        1. Populate a list of feeds                     | Would rely on MX & a common
            1.1. Show status of each feed               | list of Feeds.
            1.2. Feeds may be slected & edited          |
            1.3. A specific feed may be downloaded      |

        2. Select a subset of feeds                     | Handled by SZ
            2.1. Delete subset                          |
            2.2. Process subset                         |
            2.3. Invert selection                       |

        3. [1.2.] Edit feeds                            | Every entry that is being
            3.1. Change Title, Url, Expression through  | edited, or has edits stored
                text entry                              | for it, will have a copy made.
            3.2. Partial fetch where the feed is        | When changes are conf, copy
                downloaded and a list of titles & dates | overrides original.
                are stored to allow user to set history | SZ->copies, ref Feeds->MX

        4. Display summary / stats (bottom bar + popup?)
            4.1. Show event summary information
                - feeds downloaded
                - files downloaded
                - session time / breakdown of how long
                    each step took?
                - failed downloads
                - show box & whisker plot / data itself
                    for file sizes, download times, &c

        5. misc
            5.1. Customization (background/font/border)
            5.2. Scheduled run? / Minimze to task bar to run in background
            where it checks for new downloads (similar to a cron job)
            5.3. Custom REST api to control the process
            5.4. Customizable outgoing REST/RPC to control other processes
            based on this one (tranmission)
        ------------------------------------------------------------------------------*/
    }
}