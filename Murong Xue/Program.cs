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
    static readonly int MINOR_VERSION = 0;
    static readonly int PATCH = 0;
    //---
    static readonly Config cfg = Config.GetInstance();
    static readonly Reporter report = new(LogFlag.DEFAULT, "PROGRAM");
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
        report.Log(LogFlag.DEBUG, $"Started program with args: {args}");
#if DEBUG
        //args = ["--edit",];
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
        report.Log(LogFlag.DEBUG, "Program STOP");
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

    protected static readonly string[] help_cmds = { "-help", "--help", "-h", "--h"};
    protected static readonly string[] version_cmds = { "--version", "-version", "-v", "--v" };
    protected static readonly string[] edit_cmds = { "-edit", "--edit" };
    protected static readonly string[] rss_cmds = { "-rsscfg", "--rsscfg" };
    protected static readonly string[] download_cmds = { "-downloadpath", "--downloadpath" };
    protected static ArgResult HandleArgs(string[] args)
    {
        bool NextIsConfig = false;
        bool NextIsDownloadDir = false;
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
                    else if (NextIsDownloadDir)
                    {
                        report.Log(LogFlag.DEBUG, $"RSSCFG: Set Path to {arg}");
                        cfg.SetDownloadPath(Path.GetFullPath(arg));
                //---
                NextIsDownloadDir = false;
                continue;
                    }
            //-------------------------
            string _arg = arg.ToLower();
            if (help_cmds.Contains(_arg))
            {
                report.Log(LogFlag.DEBUG, "HELP");
                report.Log(LogFlag.FEEDBACK,
                   "Use -rsscfg PATH\\CONFIGFILE.xml to set the confit path\n" +
                    "Use -downloadPath PATH\\DOWNLOADS_FOLDER to set the downloads folder\n" +
                    "Use -version to get the last commit hash(gotta remember to update this one)");
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
        }
        //---
        report.Log(LogFlag.DEBUG, $"HandleArgs returning: {retVal}");
        return retVal;
    }
}