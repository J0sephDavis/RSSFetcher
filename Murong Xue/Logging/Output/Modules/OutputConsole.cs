using RSSFetcher.Logging;

namespace RSSFetcher.Logging.Output.Modules
{

    public interface IOutputConsole
    {
        void Pause();
        void Unpause();
        bool IsPaused();
        bool IsInteractive();
    }

    public class LogConsole() : IOutputModule, IOutputConsole
    {
        public OutputModuleClassification Type { get => OutputModuleClassification.CONSOLE; }
        public void WriteMsg(LogMsg msg)
        {
            Console.WriteLine(msg.ToString());
        }
        public void Dispose() { }
        public void Pause() { }
        public void Unpause() { }
        public bool IsPaused() { return false; }
        public bool IsInteractive() => false;
    }

    public class InteractiveConsole() : IOutputModule, IOutputConsole
    {
        public bool IsInteractive() => true;
        public OutputModuleClassification Type { get => OutputModuleClassification.CONSOLE; }
        /* diregard all print statements tagged with INTERACTIVE
         * msgs tagged with INTERACTIVE are assumed to have been sent through the interactive
         * reporter and thus have their own pathway to display
         */
        /* 1. report.Interactive(foo)
         * 1.1. IMMEDIATELY display in console -> WriteMsg
         * 1.2. Send through the normal channels to replicate in all logs
         * 2. Pause()
         * 2.1. Places all msgs not written through Interactive in a buffer
         * 3. Unpause()
         * 3.1. Unpauses output and prints buffer
         * ### must block the acceptance of any message tagged INTERACTIVE
         * (because we likely already handled it)
         */
        // ---
        private readonly object tv_remote = new();
        private bool Paused = false;
        public void Pause()
        {
            lock (tv_remote) Paused = true;
        }
        public void Unpause()
        {
            PrintBuff();
            lock (tv_remote) Paused = false;
        }
        public bool IsPaused()
        {
            lock (tv_remote) return Paused;
        }
        // ---
        private readonly object bufflock = new();
        private readonly List<LogMsg> buffer = [];
        private void PrintBuff()
        {
            lock (bufflock)
            {
                foreach (var msg in buffer)
                {
                    DoMsg(msg);
                }
                buffer.Clear();
            }
        }
        // ---
        private void DoMsg(LogMsg msg)
        {
            if ((msg & LogMod.INTERACTIVE) != 0)
                Console.WriteLine(msg.ToInteractiveString());
            else if ((msg & LogType.ERROR) != 0)
                Console.WriteLine(msg);
            else
                Console.WriteLine("REDACTED");
        }
        public void WriteMsg(LogMsg msg)
        {
            lock(tv_remote)
            {
                if (Paused)
                {
                    lock (bufflock)
                        buffer.Add(msg);
                }
                else
                    DoMsg(msg);
            }
        }
        public void Dispose() { }
    }
}
