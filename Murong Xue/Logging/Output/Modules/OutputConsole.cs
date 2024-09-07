using RSSFetcher.Logging;

namespace Murong_Xue.Logging.Output.Modules
{
    public class LogConsole() : IOutputModule
    {
        //rcvs msg
        public void WriteMsg(LogMsg msg)
        {
            Console.WriteLine(msg.ToString());
        }
        public void Dispose() { }
    }
    public class InteractiveConsole() : IOutputModule
    {
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
            lock (tv_remote) Paused = false;
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
                    if ((msg & LogMod.INTERACTIVE) == 0)
                        Console.WriteLine(msg);
                    else
                        Console.WriteLine("REDACTED");
                }
            }
        }
        // ---
        public void WriteMsg(LogMsg msg)
        {
            if (Paused)
            {
                lock (bufflock)
                    buffer.Add(msg);
            }
            else
                Console.WriteLine(msg);
        }
        public void Dispose() { }
    }
}
