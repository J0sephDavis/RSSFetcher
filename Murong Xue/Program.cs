namespace Murong_Xue;
public class Program
{
    static readonly int MAJOR_VERSION = 1;
    static readonly int MINOR_VERSION = 4; //commit 142
    static readonly int PATCH = 36;
    //---
    static Reporter report;
    static EntryData? RSSEntries = null;
    static EventTicker events = EventTicker.GetInstance();

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
        using Config cfg = Config.GetInstance();
        report ??= Config.OneReporterPlease("PROGRAM");
        report.Debug($"VERSION {MAJOR_VERSION}.{MINOR_VERSION}.{PATCH}");
        report.Trace($"Started program with {args.Length}args");
#if DEBUG
        report.Debug("!! PROGRAM COMPILED IN DEBUG MODE !!");
        //Config.SetLogLevel(LogType.ALL, LogMod.ALL);
        args = [
            "--edit"
            //"--help"
            //"--SPAM",
            //"--VERBOSE",
            //"--UNIMPORTANT",
            //"--DEBUG",
            //"--edit",
        ];
#endif
        foreach (string s in args)
            report.TraceVal(s);
        ArgResult choice = HandleArgs(cfg, args);

        report.Notice("Program starting");
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
                goto case ArgResult.EXIT;
            case (ArgResult.EXIT):
                report.Trace("ArgResult.EXIT");
                return;
        }
        report.Out(events.GetSummary());
        report.Notice("Program STOP");
    }
    public static async Task StartEditor()
    {
        if (RSSEntries == null)
        {
            report.Error("RSSEntries == NULL");
            return;
        }
        //---
        InteractiveEditor editor = new(RSSEntries.GetFeeds());
        if (editor.MainLoop())
        {
            report.Out("Saving entries from interactive session");
            RSSEntries.UpdateEntries();
        }
        else
            report.Out("Discarding change from interactive session");
    }

    private static ArgResult HandleArgs(Config cfg, string[] args)
    {
        bool NextIsConfig = false;
        bool NextIsDownloadDir = false;
        bool SetLogLevel = false;
        LogLevel level = new(LogType.DEFAULT, LogMod.DEFAULT);
        ArgResult retVal = ArgResult.RUN;
        report.Notice("Processing Args");
        foreach (string arg in args)
        {
            if (NextIsConfig)
            {
                cfg.SetRSSPath(Path.GetFullPath(arg));
                //---
                NextIsConfig = false;
                continue;
            }
            if (NextIsDownloadDir)
            {
                cfg.SetDownloadPath(Path.GetFullPath(arg));
                //---
                NextIsDownloadDir = false;
                continue;
            }
            //-------------------------
            string _arg = arg.ToLower();
            if (help_cmds.Contains(_arg))
            {
                report.Out(help_str);
                retVal = ArgResult.EXIT;
                continue;
            }
            if (version_cmds.Contains(_arg))
            {
                report.Out($"VERSION {MAJOR_VERSION}.{MINOR_VERSION}.{PATCH}");
                retVal = ArgResult.EXIT;
                continue;
            }
            if (edit_cmds.Contains(_arg))
            {
                retVal = ArgResult.EDIT;
                continue;
            }
            //-------------------------
            switch (_arg)
            {
                case ("--spam"):
                    level |= LogMod.SPAM;
                    SetLogLevel = true;
                    break;
                case ("--verbose"):
                    level |= LogMod.VERBOSE;
                    SetLogLevel = true;
                    break;
                case ("--unimportant"):
                    level |= LogMod.UNIMPORTANT;
                    SetLogLevel = true;
                    break;
                case ("--debug"):
                    level |= LogType.DEBUG;
                    break;
                default:
                    break;
            }
            //-------------------------
            if (rss_cmds.Contains(_arg))
            {
                NextIsConfig = true;
                continue;
            }
            if (download_cmds.Contains(_arg))
            {
                NextIsDownloadDir = true;
                continue;
            }
        }
        //---
        if (SetLogLevel)
        {
            report.Out($"Masking log level {level}");
            Config.MaskLogLevel(level);
        }
        report.Log(LogType.DEBUG, LogMod.UNIMPORTANT, $"HandleArgs returning: {retVal}");
        return retVal;
    }
    //---------- STATIC STRINGS -----------
    protected static readonly string[] version_cmds = ["--version", "-version", "-v", "--v"];
    protected static readonly string version_cmd_desc = "(void) Show version information";

    protected static readonly string[] edit_cmds = ["-edit", "--edit"];
    protected static readonly string edit_cmd_desc = "(void) Enter interactive mode and edit the rss config";

    protected static readonly string[] rss_cmds = ["-rsscfg", "--rsscfg"];
    protected static readonly string rss_cmd_desc = "(string) Set the path of the RSS Config file";

    protected static readonly string[] download_cmds = ["-downloadpath", "--downloadpath"];
    protected static readonly string download_cmd_desc = "(string) Set the download folder for all fetched files";

    protected static readonly string log_cmd_desc = "Enable the following log flags:" +
        "\n\t--SPAM" +
        "\n\t--VERBOSE" +
        "\n\t--UNIMPORTANT" +
        "\n\t--DEBUG";

    protected static readonly string[] help_cmds = ["-help", "--help", "-h", "--h"];
    protected static readonly string help_cmd_desc = "(void) Get a brief description for each command";
    protected static readonly string help_str =
        $"{help_cmds[0]}:\t{help_cmd_desc}\n" +
        $"{version_cmds[0]}:\t{version_cmd_desc}\n" +
        $"{edit_cmds[0]}:\t{edit_cmd_desc}\n" +
        $"{rss_cmds[0]}:\t{rss_cmd_desc}\n" +
        $"{download_cmds[0]}:\t{download_cmd_desc}\n" +
        $"{log_cmd_desc}";
}