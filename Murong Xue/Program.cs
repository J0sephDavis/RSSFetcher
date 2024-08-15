/* TODO
 * Create a cron/headless option that will minimize output to include:
 * 1. Total files scanned
 * 2. Total files added/fetched
 * 3. Running time
*/
namespace Murong_Xue;
public class Program
{
    static readonly int MAJOR_VERSION = 1;
    static readonly int MINOR_VERSION = 2;
    static readonly int PATCH = 100;
    //---
    static readonly Config cfg = Config.GetInstance();
    static Reporter report;
    static EntryData? RSSEntries = null;
    [Flags]
    protected enum ArgResult
    {
        NONE    = 0,
        EXIT    = 1 << 0,
        EDIT    = 1 << 1,
        RUN     = 1 << 2,
    };
    public static async Task Main(string[] args)
    {
        report ??= Config.OneReporterPlease("PROGRAM");
        report.Log(LogFlag.DEBUG, $"Started program with {args.Length}args");
        foreach (string s in args)
            report.Log(LogFlag.DEBUG_SPAM, s);
#if DEBUG
        report.Log(LogFlag.WARN, "!!!PROGRAM COMPILED IN DEBUG MODE!!!");
        args = [
            "--loglevel",
            //$"{32}",
            $"{((int)(LogFlag.DEBUG | LogFlag.FEEDBACK | LogFlag._EXCEPTION | LogFlag.NOTEWORTHY))}",
            //"--help"
        ];
#endif
        ArgResult choice = HandleArgs(args);

        report.Log(LogFlag.DEBUG, "Program starting");
        RSSEntries = new EntryData(cfg.GetRSSPath());
        switch (choice)
        {
            case (ArgResult.EDIT):
                await StartEditor();
                break;
            case (ArgResult.RUN):
                await RSSEntries.Process();
                break;
            case (ArgResult.NONE):
            default:
                report.Log(LogFlag.WARN | LogFlag.DEBUG, "DEFFAULT/ARG.RESLT = NONE. EXITING");
                goto case ArgResult.EXIT;
            case (ArgResult.EXIT):
                report.Log(LogFlag.DEBUG, "ArgResult.EXIT, Quitting.");
                return;
        }
        report.Log(LogFlag.FEEDBACK, "Program STOP");
        Logger.Quit();
    }
    public static async Task StartEditor()
    {
        if (RSSEntries == null)
        {
            report.Log(LogFlag.ERROR, "RSSEntries is NULL");
            return;
        }
        //---
        InteractiveEditor editor = new(RSSEntries.GetFeeds());
        if (editor.MainLoop())
        {
            report.Log(LogFlag.FEEDBACK, "Saving entries from interactive session");
            await RSSEntries.UpdateEntries();
        }
        else
            report.Log(LogFlag.FEEDBACK, "Discarding change from interactive session");
    }

    protected static readonly string[] version_cmds = ["--version", "-version", "-v", "--v"];
    protected static readonly string version_cmd_desc = "(void) Show version information";

    protected static readonly string[] edit_cmds = ["-edit", "--edit"];
    protected static readonly string edit_cmd_desc = "(void) Enter interactive mode and edit the rss config";

    protected static readonly string[] rss_cmds = ["-rsscfg", "--rsscfg"];
    protected static readonly string rss_cmd_desc = "(string) Set the path of the RSS Config file";

    protected static readonly string[] download_cmds = ["-downloadpath", "--downloadpath"];
    protected static readonly string download_cmd_desc = "(string) Set the download folder for all fetched files";

    protected static readonly string[] log_cmds = ["--loglevel", "-loglevel", "-log", "--log", "--level", "-level"];
    protected static readonly string log_cmd_desc = "(int) Set the log level:\n" + //regex for converting the enum into this FIND:\s+(\w+).* REPLACE:\$\"$1\({LogFlag.$1}\)\\t\" +\n
        $"\tNONE({(int)LogFlag.NONE})\t\t" +
            $"DEBUG_SPAM({(int)LogFlag.DEBUG_SPAM})\n" +
        $"\tDEBUG({(int)LogFlag.DEBUG})\t" +
            $"SPAM({(int)LogFlag.SPAM})\n" +
        $"\tWARN({(int)LogFlag.WARN})\t\t" +
            $"NOTEWORTHY({(int)LogFlag.NOTEWORTHY})\n" +
        $"\tFEEDBACK({(int)LogFlag.FEEDBACK})\t" +
            $"ERROR({(int)LogFlag.ERROR})\n" +
        "\t------- Derived Flags -------\n" +
        $"\t_EXCEPTION({(int)LogFlag._EXCEPTION})\t" +
            $"_DEBUG({(int)LogFlag._DEBUG})\n" +
        $"\t_INFO({(int)LogFlag._INFO})\t" +
            $"DEFAULT({(int)LogFlag.DEFAULT})\n" +
        $"\tDEFAULT({(int)LogFlag.DEFAULT})\t" +
            $"HEADLESS({(int)LogFlag.HEADLESS})\n" +
        $"\tALL({(int)LogFlag.ALL})";

    protected static readonly string[] help_cmds = ["-help", "--help", "-h", "--h"];
    protected static readonly string help_cmd_desc = "(void) Get a brief description for each command";
    protected static readonly string help_str =
        $"{help_cmds[0]}:\t{help_cmd_desc}\n" +
        $"{version_cmds[0]}:\t{version_cmd_desc}\n" +
        $"{edit_cmds[0]}:\t{edit_cmd_desc}\n" +
        $"{rss_cmds[0]}:\t{rss_cmd_desc}\n" +
        $"{download_cmds[0]}:\t{download_cmd_desc}\n" +
        $"{log_cmds[0]}:\t{log_cmd_desc}";
    protected static ArgResult HandleArgs(string[] args)
    {
        bool NextIsConfig = false;
        bool NextIsDownloadDir = false;
        bool NextIsLogLevel = false;
        ArgResult retVal = ArgResult.RUN;

        report.Log(LogFlag.DEBUG, "Processing args");
        foreach (string arg in args)
        {
            if (NextIsConfig)
            {
                report.Log(LogFlag.DEBUG, $"RSSCFG: Set Path to {arg}");
                cfg.SetRSSPath(Path.GetFullPath(arg));
                //---
                NextIsConfig = false;
                continue;
            }
            if (NextIsDownloadDir)
            {
                report.Log(LogFlag.DEBUG, $"RSSCFG: Set Path to {arg}");
                cfg.SetDownloadPath(Path.GetFullPath(arg));
                //---
                NextIsDownloadDir = false;
                continue;
            }
            if (NextIsLogLevel)
            {
                LogFlag value = (LogFlag)int.Parse(arg); //TODO exception handling // TryParse
                report.Log(LogFlag.DEBUG, $"LOGLEVEL: Set to {value}");
                Config.SetLogLevel(value);
                NextIsLogLevel = false;
                continue;
            }
            //-------------------------
            string _arg = arg.ToLower();
            if (help_cmds.Contains(_arg))
            {
                report.Log(LogFlag.DEBUG, "HELP");
                report.Log(LogFlag.FEEDBACK,
                   help_str);
                retVal = ArgResult.EXIT;
                continue;
            }
            if (version_cmds.Contains(_arg))
            {
                report.Log(LogFlag.DEBUG, "VERSION");
                report.Log(LogFlag.FEEDBACK, $"VERSION {MAJOR_VERSION}.{MINOR_VERSION}.{PATCH}");
                retVal = ArgResult.EXIT;
                continue;
            }
            if (edit_cmds.Contains(_arg))
            {
                report.Log(LogFlag.DEBUG, "EDIT: EditConfigs = true");
                retVal = ArgResult.EDIT;
                continue;
            }
            //-------------------------
            if (rss_cmds.Contains(_arg))
            {
                report.Log(LogFlag.DEBUG, "RSSCFG: NextIsConfig");
                NextIsConfig = true;
                continue;
            }
            if (download_cmds.Contains(_arg))
            {
                report.Log(LogFlag.DEBUG, "DLPATH: NextIsPath");
                NextIsDownloadDir = true;
                continue;
            }
            if (log_cmds.Contains(_arg))
            {
                report.Log(LogFlag.DEBUG, "LOG LEVEL: NextIsLogLevel");
                NextIsLogLevel = true;
                continue;
            }
        }
        //---
        report.Log(LogFlag.DEBUG, $"HandleArgs returning: {retVal}");
        return retVal;
    }
}