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
    static readonly int MINOR_VERSION = 3; //commit 111
    static readonly int PATCH = 13;
    //---
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
        using (Config cfg = Config.GetInstance())
        {
            report ??= Config.OneReporterPlease("PROGRAM");
            report.Trace($"Started program with {args.Length}args");
            foreach (string s in args)
                report.Trace(s);
#if DEBUG
            report.Notice("PROGRAM COMPILED IN DEBUG MODE");
            args.Append("--help");
            //args.Append($"--loglevel {LogFlag.ALL}");
#endif
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
                    report.Trace("DEFFAULT/ARG.RESLT = NONE. EXITING");
                    goto case ArgResult.EXIT;
                case (ArgResult.EXIT):
                    report.Trace("ArgResult.EXIT");
                    return;
            }
            report.Notice("Program STOP");
        }
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
            await RSSEntries.UpdateEntries();
        }
        else
            report.Out("Discarding change from interactive session");
    }

    private static ArgResult HandleArgs(Config cfg, string[] args)
    {
        bool NextIsConfig = false;
        bool NextIsDownloadDir = false;
        bool NextIsLogLevel = false;
        ArgResult retVal = ArgResult.RUN;
        report.Notice("Processing Args");
        foreach (string arg in args)
        {
            if (NextIsConfig)
            {
                report.Trace("Setting rss path");
                cfg.SetRSSPath(Path.GetFullPath(arg));
                //---
                NextIsConfig = false;
                continue;
            }
            if (NextIsDownloadDir)
            {
                report.Trace("Setting download path");
                cfg.SetDownloadPath(Path.GetFullPath(arg));
                //---
                NextIsDownloadDir = false;
                continue;
            }
            if (NextIsLogLevel)
            {
                /*LogFlag value = (LogFlag)int.Parse(arg); //TODO exception handling // TryParse
                report.Trace("Setting log level");
                Config.SetLogLevel(value);*/
                report.Error("LOG LEVEL HANDLING WIP");
                NextIsLogLevel = false;
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
                report.Trace("EditConfigs = true");
                retVal = ArgResult.EDIT;
                continue;
            }
            //-------------------------
            if (rss_cmds.Contains(_arg))
            {
                report.Trace("NextIsConfig = true");
                NextIsConfig = true;
                continue;
            }
            if (download_cmds.Contains(_arg))
            {
                report.Trace("NextIsDownloadDir = true");
                NextIsDownloadDir = true;
                continue;
            }
            if (log_cmds.Contains(_arg))
            {
                report.Trace("NextIsLogLevel = true");
                NextIsLogLevel = true;
                continue;
            }
        }
        //---
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

    protected static readonly string[] log_cmds = ["--loglevel", "-loglevel", "-log", "--log", "--level", "-level"];
    protected static readonly string log_cmd_desc = "(int) Set the log level:\n" + //regex for converting the enum into this FIND:\s+(\w+).* REPLACE:\$\"$1\({LogFlag.$1}\)\\t\" +\n
        @"//---
        NONE = 0,
        NORMAL = 0,
        //--- Modifiers
        SPAM            = 1 << 0,
        VERBOSE         = 1 << 1,
        UNIMPORTANT     = 1 << 2,
        INTERACTIVE     = 1 << 3,
        
        _ALL_MODS = SPAM | VERBOSE | UNIMPORTANT | NORMAL,
        //--- Types
        DEBUG           = 1 << 4,
        ERROR           = 1 << 5,
        OUTPUT          = 1 << 6,

        _ALL_TYPES = DEBUG | ERROR | OUTPUT,
        //Derived debug
        DEBUG_SPAM      = DEBUG | SPAM,
        DEBUG_WARN      = DEBUG | WARN,
        DEBUG_OBLIG     = DEBUG | UNIMPORTANT,
        DEBUG_SETVAL    = DEBUG | VERBOSE | SPAM,
        //Derived error
        WARN            = ERROR | UNIMPORTANT,
        //Derived output
        CHATTER         = OUTPUT | UNIMPORTANT,

        //---
        _DEFAULT_MODS   = NORMAL,
        DEFAULT = OUTPUT | ERROR | _DEFAULT_MODS,
        ALL = _ALL_MODS | _ALL_TYPES,";// ....

    protected static readonly string[] help_cmds = ["-help", "--help", "-h", "--h"];
    protected static readonly string help_cmd_desc = "(void) Get a brief description for each command";
    protected static readonly string help_str =
        $"{help_cmds[0]}:\t{help_cmd_desc}\n" +
        $"{version_cmds[0]}:\t{version_cmd_desc}\n" +
        $"{edit_cmds[0]}:\t{edit_cmd_desc}\n" +
        $"{rss_cmds[0]}:\t{rss_cmd_desc}\n" +
        $"{download_cmds[0]}:\t{download_cmd_desc}\n" +
        $"{log_cmds[0]}:\t{log_cmd_desc}";
}