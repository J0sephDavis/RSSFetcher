using RSSFetcher.Logging;

namespace RSSFetcher.Tests;
[TestClass]
public class CommandLineArgumentsTest
{
    public TestContext testContext { get => testContext; set => testContext = value; }
    // PREFIX
    string[] cmd_prefix = ["-", "--"];
    // SET FOLDERS (1 ARG)
    string[] cmd_download = ["downloadpath", "downloadPath", "DOWNLOADPATH", "downloadPATH"];
    string[] cmd_rss = ["rsscfg", "rssCFG", "RSSCFG"];
    // MODE
    string[] cmd_edit = ["edit", "Edit", "EDIT", "EDIt"];
    string[] cmd_version = ["version", "Version", "VERSION", "VERSIOn"];
    string[] cmd_help = ["help", "HELP","h","H"];
    // LOGGING
    string[] cmd_spam = ["spam","Spam","SPAM","spam"];
    string[] cmd_debug = ["debug","Debug","DEBUG"];
    string[] cmd_verbose = ["verbose", "VERBOSE", "Verbose"];
    // ---
    const string rss_file = "rss-config.xml";
    const string download_folder = "Downloads";
    const string log_file = "RSS-F.log";

    static readonly string AppRootDirectory
        = Path.GetDirectoryName(System.AppContext.BaseDirectory)
        + Path.DirectorySeparatorChar;

    readonly Uri DefaultLogUri = new(AppRootDirectory + log_file);
    readonly Uri DefaultDownloadUri = new(AppRootDirectory + download_folder + Path.DirectorySeparatorChar);
    readonly Uri DefaultRSSUri = new(AppRootDirectory + rss_file);

    CommandLineArguments defaultConstructor() => new([string.Empty]);
    CommandLineArguments argConstructor(string s1, string s2) => new ([s1,s2]);
    #region RSSPath
    [TestMethod]
    public void  DefaultRSSPath()
    {
        Assert.AreEqual(defaultConstructor().RSSPath.LocalPath, DefaultRSSUri.LocalPath);
    }
    [TestMethod]
    public void SetRSSPath()
    {
        for (int i = 0; i < cmd_rss.Length; i++)
        {
            for (int j = 0; j < cmd_prefix.Length; j++)
            {
                CommandLineArguments cla = new([cmd_prefix[j], cmd_rss[i], DefaultRSSUri.OriginalString]);
                Assert.AreEqual(cla.RSSPath.LocalPath, DefaultRSSUri.LocalPath);
            }
        }
    }
    #endregion
    #region DownloadDirectory
    [TestMethod]
    public void DefaultDownloadDirectory()
    {
        Assert.AreEqual(defaultConstructor().DownloadDirectory.LocalPath, DefaultDownloadUri.LocalPath);
    }
    [TestMethod]
    public void SetDownloadPath()
    {
        for (int i = 0; i < cmd_download.Length; i++)
        {
            for (int j = 0; j < cmd_prefix.Length; j++)
            {
                CommandLineArguments cla = new([cmd_prefix[j], cmd_download[i], DefaultDownloadUri.OriginalString]);
                Assert.AreEqual(cla.DownloadDirectory.LocalPath, DefaultDownloadUri.LocalPath);
            }
        }
    }
    #endregion
    #region LogFile
    [TestMethod]
    public void DefaultLogFile()
    {
        Assert.AreEqual(defaultConstructor().LogPath.LocalPath, DefaultLogUri.LocalPath);
    }
    /* not imlpemented yet
    [TestMethod]
    public void SetDownloadPath()
    {
        for (int i = 0; i < cmd_download.Length; i++)
        {
            for (int j = 0; j < cmd_prefix.Length; j++)
            {
                CommandLineArguments cla = new([cmd_prefix[j], cmd_logfile[i], DefaultLogUri.OriginalString]);
                Assert.AreEqual(cla.DefaultLogUri.LocalPath, DefaultLogUri.LocalPath);
            }
        }
    }*/
    #endregion
    #region result
    [TestMethod]
    public void DefaultArgResult() => Assert.AreEqual(defaultConstructor().Result, ArgResult.RUN);
    [TestMethod]
    public void ArgResult_EDIT()
    {
        CheckResult(cmd_edit, ArgResult.EDIT);
    }
    [TestMethod]
    public void ArgResult_EXIT()
    {
        CheckResult(cmd_version, ArgResult.EXIT);
        CheckResult(cmd_help, ArgResult.EXIT);
    }

    void CheckResult(string[] cmd_arr, ArgResult expected)
    {
        foreach (var cla_version in GetVariants(GetVariants(cmd_arr)))
        {
            Assert.AreEqual(cla_version.Result, expected);
        }
    }
    #endregion
    #region loglevels
    LogLevel defaultLevels = new(LogType.DEFAULT, LogMod.DEFAULT);
    [TestMethod]
    public void LogLevel_SPAM()
    {
        LogLevel spamLevel = defaultLevels | LogMod.SPAM;
        //log levels don't support multi-prefixes atm TODO tho
        foreach (string spam in cmd_spam)
            CheckLogLevel(["--" + spam], spamLevel);

        /*foreach(string spam in GetVariants(cmd_spam))
        {
            //testContext.WriteLine($"args-> {spam}");
            CheckLogLevel([spam, string.Empty], spamLevel);
        }*/
    }
    void CheckLogLevel(string[] args, LogLevel expected)
    {
        CommandLineArguments cla = new(args);
        Assert.AreEqual(cla.UT_loglevel, expected);
    }
    #endregion
    List<CommandLineArguments> GetVariants(List<string> commands, List<string> values)
    {
        List<CommandLineArguments> response = [];
        foreach (var command in commands)
            foreach (var val in values)
                response.Add(argConstructor(command, val));
        return response;
    }
    List<CommandLineArguments> GetVariants(List<string> commands) => GetVariants(commands, "");
    List<CommandLineArguments> GetVariants(List<string> commands, string value)
    {
        List<CommandLineArguments> response = [];
        foreach (var command in commands)
            response.Add(new([command]));
        return response;
    }
    List<string> GetVariants(string[] cmd)
    {
        List<string> variants = [];
        for (int i = 0; i < cmd.Length; i++)
            for (int j = 0; j < cmd_prefix.Length; j++)
                variants.Add(cmd_prefix[j] + cmd[i]);
        return variants;
    }
}