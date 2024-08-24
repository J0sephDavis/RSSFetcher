namespace Murong_Xue.Logging.OutputHandling
{
    internal interface IOutputModule
    {
        public void WriteMsg(LogMsg msg, bool InteractiveMode); //accept a copy of the buffer?
        public void Dispose();
    }
    internal class LogConsole : IOutputModule
    {
        //rcvs msg
        public void WriteMsg(LogMsg msg, bool InteractiveMode)
        {
            Console.WriteLine(InteractiveMode ? msg.ToInteractiveString() : msg.ToString());
        }
        public void Dispose() { }
    }
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
        public void WriteMsg(LogMsg msg, bool InteractiveMode)
        {
            //ignores interactive mode
            FileStream?.Write(msg);
        }
        public void Dispose()
        {
            FileStream?.Dispose();
        }
    }
}
