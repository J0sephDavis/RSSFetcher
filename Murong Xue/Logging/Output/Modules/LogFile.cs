using RSSFetcher.Logging;

namespace Murong_Xue.Logging.Output.Modules
{
    internal class LogFile : IOutputModule
    {
        private readonly StreamWriter? FileStream;
        private readonly Uri? FilePath;
        public LogFile(Uri path)
        {
            if (path?.IsFile != true)
            {
                Logger.Log(new(LogType.ERROR, LogMod.NORMAL, "LOGPUT", "SetPath FAILED, path does not lead to file"));
                return;
            }
            Logger.Log(new(LogType.DEBUG, LogMod.SPAM | LogMod.VERBOSE, "LOGPUT", $"SetPath: [{path}]")); //traceval

            if (File.Exists(path.LocalPath))
            {
                Logger.Log(new(LogType.ERROR, LogMod.SPAM | LogMod.UNIMPORTANT, "LOGPUT", "Deleting previous logfile")); //warnspam
                File.Delete(path.LocalPath);
            }
            FilePath = path;
            FileStream = new(FilePath.LocalPath);
        }
        public void WriteMsg(LogMsg msg)
        {
            //ignores interactive mode
            FileStream?.WriteLine(msg);
        }
        public void Dispose()
        {
            FileStream?.Dispose();
        }
    }
}
