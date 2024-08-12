/* TODO
 * Create a cron/headless option that will minimize output to include:
 * 1. Total files scanned
 * 2. Total files added/fetched
 * 3. Running time
 * ----
 * This might benefit from a logging system with flags (DEBUG/ERROR/WARN/VERBOSE/NOTEWORTH/)
 */
using Murong_Xue;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml;

namespace MurongXue;
public class Program
{
    private static readonly Config cfg = Config.GetInstance();
    private static readonly Reporter report = new Reporter(LogFlag.DEFAULT, "PROGRAM");
    private static EntryData? RSSEntries = null;
    public static async Task Main(string[] args)
    {
        report.Log(LogFlag.DEBUG, $"Started program with args: {args}");
        bool NextIsConfig = false;
        bool NextIsDownloadDir = false;
        bool EditConfigs = false;
#if DEBUG
        //args = ["--edit",];
#endif
        report.Log(LogFlag.DEBUG, "Processing args");
        foreach (string arg in args)
        {
            switch (arg.ToLower())
            {
                case "-help":
                case "--help":
                case "-h":
                    report.Log(LogFlag.DEBUG, "HELP");
                    report.Log(LogFlag.FEEDBACK,
                       "Use -rsscfg PATH\\CONFIGFILE.xml to set the confit path\n" +
                        "Use -downloadPath PATH\\DOWNLOADS_FOLDER to set the downloads folder\n" +
                        "Use -version to get the last commit hash(gotta remember to update this one)");
                    return;
                case "--version":
                case "-version":
                case "--v":
                case "-v":
                    report.Log(LogFlag.DEBUG, "VERSION");
                    report.Log(LogFlag.FEEDBACK, "Last commit hash: d5b80b86");
                    return;
                //-------------------------
                case "--edit":
                case "-edit":
                    EditConfigs = true;
                    report.Log(LogFlag.DEBUG, "EDIT: EditConfigs = true");
                    break;
                //-------------------------
                case "--rsscfg":
                case "-rsscfg":
                    report.Log(LogFlag.DEBUG, "RSSCFG: NextIsConfig");
                    NextIsConfig = true;
                    break;
                case "--downloadpath":
                case "-downloadpath":
                    report.Log(LogFlag.DEBUG, "DLPATH: NextIsPath");
                    NextIsDownloadDir = true;
                    break;
                default:
                    if (NextIsConfig)
                    {
                        report.Log(LogFlag.DEBUG, $"RSSCFG: Set Path to {arg}");
                        cfg.SetRSSPath(Path.GetFullPath(arg));
                    }
                    else if (NextIsDownloadDir)
                    {
                        report.Log(LogFlag.DEBUG, $"RSSCFG: Set Path to {arg}");
                        cfg.SetDownloadPath(Path.GetFullPath(arg));
                    }
                    //-
                    report.Log(LogFlag.DEBUG, $"NextIsDL/CFG = FALSE");
                    NextIsDownloadDir = false;
                    NextIsConfig = false;
                    break;
            }
        }
        report.Log(LogFlag.DEBUG, "Program starting");
        RSSEntries = new EntryData(cfg.GetRSSPath());
        if (EditConfigs)
        {
            InteractiveEditor editor = new InteractiveEditor(RSSEntries.GetFeeds());
            if (editor.MainLoop())
            {
                report.Log(LogFlag.FEEDBACK, $"Saving entries from interactive session");
                await RSSEntries.UpdateEntries();
            }
            else
                report.Log(LogFlag.FEEDBACK, $"Discarding change from interactive session");
        }
        else
            await RSSEntries.Process();
    }
}