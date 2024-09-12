using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace RSSFetcher
{
    [Flags]
    public enum ArgResult
    {
        NONE = 0,
        EXIT = 1 << 0,
        EDIT = 1 << 1,
        RUN = 1 << 2,
    };
    internal class CommandLineArguments //TODO rename Controller Configuration? Controller Arguments?
    {
        const string rss_file = "rss-config.xml";
        const string download_folder = "Downloads";
        const string log_file = "RSS-F.log";


        public static readonly string AppRootDirectory = Path.GetDirectoryName(System.AppContext.BaseDirectory) + Path.DirectorySeparatorChar;
        Reporter report = Logger.RequestReporter("CLIARG");
        public ArgResult Result { get; private set; } = ArgResult.RUN;
        // --- eventually set by args
        public readonly Uri LogPath = new(AppRootDirectory + "RSS-F.log");
        // --- can be set by command line!!!
        public readonly Uri DownloadDirectory = new(AppRootDirectory + @"Downloads" + Path.DirectorySeparatorChar);
        // --- can be set by command line!!!
        public readonly Uri RSSPath = new(AppRootDirectory + "rss-config.xml");

        public CommandLineArguments(string[] args)
        {
            report.Notice("Processing Args");
            // ---
#if DEBUG
            report.Trace($"HandleArgs provided with {args.Length}args");
            foreach (string s in args)
                report.TraceVal(s);
            report.TraceVal($"result: RootDir {RSSPath}\n\tDownload DIR {DownloadDirectory}");
#endif
            // ---
            bool NextIsConfig = false;
            bool NextIsDownloadDir = false;
            bool SetLogLevel = false;
            // ---
            LogLevel level = new(LogType.DEFAULT, LogMod.DEFAULT);
            // ---
            foreach (string arg in args)
            {
                if (NextIsConfig)
                {
                    RSSPath = new(Path.GetFullPath(arg));
                    //---
                    NextIsConfig = false;
                    continue;
                }
                if (NextIsDownloadDir)
                {
                    DownloadDirectory = new(Path.GetFullPath(arg));
                    //---
                    NextIsDownloadDir = false;
                    continue;
                }
                //-------------------------
                string _arg = arg.ToLower();
                if (help_cmds.Contains(_arg))
                {
                    report.Out(help_str);
                    Result = ArgResult.EXIT;
                    continue;
                }
                if (version_cmds.Contains(_arg))
                {
                    report.Out(Controller.versionString);
                    Result = ArgResult.EXIT;
                    continue;
                }
                if (edit_cmds.Contains(_arg))
                {
                    Result = ArgResult.EDIT;
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
#if DEBUG
                    case ("--debug"):
                        level |= LogType.DEBUG;
                        break;
#endif
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
                Logger.MaskLogLevel(level);
            }
            report.Log(LogType.DEBUG, LogMod.UNIMPORTANT, $"HandleArgs set: {Result}");
        }
        #region Static strings for arguments
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
            "\n\t--UNIMPORTANT"
#if DEBUG
            + "\n\t--DEBUG"
#endif
            ;

        protected static readonly string[] help_cmds = ["-help", "--help", "-h", "--h"];
        protected static readonly string help_cmd_desc = "(void) Get a brief description for each command";
        protected static readonly string help_str =
            $"{help_cmds[0]}:\t{help_cmd_desc}\n" +
            $"{version_cmds[0]}:\t{version_cmd_desc}\n" +
            $"{edit_cmds[0]}:\t{edit_cmd_desc}\n" +
            $"{rss_cmds[0]}:\t{rss_cmd_desc}\n" +
            $"{download_cmds[0]}:\t{download_cmd_desc}\n" +
            $"{log_cmd_desc}";
        #endregion
    }
}