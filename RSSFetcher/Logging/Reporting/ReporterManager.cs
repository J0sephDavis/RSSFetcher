using RSSFetcher.Logging.Output.Modules;

namespace RSSFetcher.Logging.Reporting
{
    /// <summary>
    /// Responsible for
    /// 1. Creating reporters (factory)
    /// 2. Propagating information to reporters
    ///     - Such as a new logging level
    ///     - If we ever use individual buffers for each reporter, it would be possible to flush all the reporters buffers here
    /// </summary>
    public class ReporterManager(LogLevel _level)
    {
        LogLevel Level = _level;
        private readonly List<Reporter> reporters = [];
        public Reporter GetReporter(string name, LogType type = LogType.NONE, LogMod mod = LogMod.NONE)
        {
            LogLevel _level = Level | mod | type;
            lock (reporters)
            //https://stackoverflow.com/questions/266681/should-a-return-statement-be-inside-or-outside-a-lock
            {
                Reporter _r = new(_level, name);
                Subscribe(_r);
                return _r;
            }
        }
        public InteractiveReporter GetInteractiveReporter(string name, LogType type = LogType.NONE, LogMod mod = LogMod.NONE)
        {
            LogLevel _level = Level | mod | type;
            lock (reporters)
            {
                InteractiveReporter _r = new(_level, name);
                Subscribe(_r);
                return _r;
            }

        }
        private void Subscribe(Reporter reporter)
        {
            lock (reporters)
            {
                reporters.Add(reporter);
            }
        }
        private void NotifySubscribers()
        {
            lock (reporters)
            {
                foreach (var r in reporters)
                {
                    r.SetLogLevel(Level);
                }
            }
        }

        public void SetLogLevel(LogLevel _level)
        {
            Level = _level;
            NotifySubscribers();
        }
        public void MaskLogLevel(LogLevel _level)
        {
            Level |= _level;
        }
    }
}
