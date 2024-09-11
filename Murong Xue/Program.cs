using RSSFetcher.Logging;
using RSSFetcher.Logging.Reporting;

namespace RSSFetcher;
public partial class Program
{
    public static readonly int MAJOR_VERSION = 1;
    public static readonly int MINOR_VERSION = 7;
    public static readonly int PATCH = 0;
    //---
    static Reporter report;
    public static void Main(string[] args)
    {
        report = Logger.RequestReporter("PROGRM");
        report.Notice($"VERSION {MAJOR_VERSION}.{MINOR_VERSION}.{PATCH}");
#if DEBUG
        report.Trace("compilation flag DEBUG was set");
        Logger.SetLogLevel(new(LogType.ALL, LogMod.ALL));
        args = [
            //"--version"
            //"--help"
            //"--SPAM",
            //"--VERBOSE",
            //"--UNIMPORTANT",
            //"--DEBUG",
            "--edit",
        ];
#endif
        using Controller controller = new(new(args));
        controller.Run();
        report.Out(controller.GetSummary());
        report.Notice("Program STOP");
    }
}