using System.Runtime.CompilerServices;

namespace Murong_Xue.Logging
{
    internal class Logger
    {
        /* After several iterations and changes on this class, it can only be considered sloppy.
         * Should it be a static class?
         * Should it be a singleton?
         * How to ensure this is cleaned up on exit?
         *  -> Included in the Config.Dipose() (because Garbage collection may not/wont happen)
         */
        private static readonly List<LogMsg> bufferedMsgs = [];
        private static readonly object buffLock = new();
        private static Logger? s_Logger = null;
        private static Reporter report = Config.OneReporterPlease("LOGGER");
        //----
        private readonly Thread batchThread;
        private static readonly AutoResetEvent batchEvent = new(false);
        private bool StopLoop = false;
        private bool InteractiveMode = false;
        //----
        const int BUFFER_THRESHOLD = 10; // msgs
        const int BUFFER_TIMEOUT = 5; // seconds
        //----
        private Uri? filePath;
        public static Logger GetInstance(string? path = null)
        {
            s_Logger ??= new Logger(path == null ? null : new Uri(path));
            return s_Logger;
        }
        private Logger(Uri? path = null)
        {
            filePath = path;
            if (path?.IsFile == true)
            {
                report.TraceVal($"Log file: [{filePath}]");
                if (File.Exists(path.LocalPath))
                {
                    report.WarnSpam("Deleting previous logfile");
                    File.Delete(path.LocalPath);
                }
                batchThread = new(BatchLoopFile);
            }
            else
            {
                Log(new LogMsg(LogType.ERROR, LogMod.NORMAL, "Logger", $"Path does not lead to a file:\n\t[{(path == null ? "null" : path.LocalPath)}]"));
                batchThread = new(BatchLoop);
            }
            batchThread.Start();
        }
        ~Logger() { Quit(); } //Probably won't be called because its the last thing to be done. GC won't take place
        public void Quit()
        {
            if (batchThread.IsAlive)
            {
                StopLoop = true;
                batchEvent.Set();
                batchThread.Join();
            }
        }
        //------------------------------------
        /// <summary>
        /// Add message to the log
        /// </summary>
        /// <param name="msg">The message</param>
        public static void Log(LogMsg msg)
        {
            lock (buffLock)
            {
                bufferedMsgs.Add(msg);
                if (bufferedMsgs.Count > BUFFER_THRESHOLD)
                    batchEvent.Set();
            }
        }

        public void SetInteractiveMode(bool val)
        {
            InteractiveMode = val;
        }
        /// <summary>
        /// The loggers main thread. Waits for the batchEvent flag to be set or BUFFER_TIMEOUT.
        /// Clears & creates a copy of the buffer and writes output to the console (WIP for files)
        /// </summary>
        private void BatchLoop() //Console only
        {
            bool changeTimeOut = !InteractiveMode;
            List<LogMsg> copiedBuff = [];
            TimeSpan waitTimeOut = TimeSpan.FromSeconds(BUFFER_TIMEOUT);

            while (StopLoop == false)
            {
                //assuming changeTimeOut is actuallythe state
                //of InteractiveMode before the loop ended
                changeTimeOut = changeTimeOut != InteractiveMode;
                if (changeTimeOut)
                {
                    if (InteractiveMode)
                        waitTimeOut = TimeSpan.FromSeconds(BUFFER_TIMEOUT / 10);
                    else
                        waitTimeOut = TimeSpan.FromSeconds(BUFFER_TIMEOUT);
                }
                changeTimeOut = InteractiveMode;
                //---
                batchEvent.WaitOne(waitTimeOut);
                //---
                lock (buffLock)
                {
                    copiedBuff = new(bufferedMsgs);
                    bufferedMsgs.Clear();
                }
                if (copiedBuff != null)
                {
                    foreach (LogMsg msg in copiedBuff)
                    {
                        if (InteractiveMode)
                        {
                            if ((msg & LogMod.INTERACTIVE) != LogMod.NONE)
                                Console.WriteLine(msg.ToInteractiveString());
                            else if ((msg & LogMod.NORMAL) != LogMod.NONE)
                                Console.WriteLine(msg);
                        }
                        else
                            Console.WriteLine(msg);
                    }
                    copiedBuff.Clear();
                }
            }
            //--- making sure the buffer is clear on exit
            lock (buffLock)
            {
                foreach (LogMsg msg in bufferedMsgs)
                    Console.WriteLine(msg);
                bufferedMsgs.Clear();
            }
        }
        private void BatchLoopFile() //file & console
        {
            using (StreamWriter file = new StreamWriter(filePath.LocalPath)) //when this is null, what happens?
            {
                bool changeTimeOut = !InteractiveMode; //force an update on first cycle
                TimeSpan waitTimeOut = TimeSpan.FromSeconds(BUFFER_TIMEOUT);
                List<LogMsg> copiedBuff = [];
                while (StopLoop == false)
                {
                    changeTimeOut = changeTimeOut != InteractiveMode;
                    if (changeTimeOut)
                    {
                        if (InteractiveMode)
                            waitTimeOut = TimeSpan.FromSeconds(BUFFER_TIMEOUT / 10);
                        else
                            waitTimeOut = TimeSpan.FromSeconds(BUFFER_TIMEOUT);
                    }
                    changeTimeOut = InteractiveMode;
                    batchEvent.WaitOne(waitTimeOut);
                    //---
                    lock (buffLock)
                    {
                        copiedBuff = new(bufferedMsgs);
                        bufferedMsgs.Clear();
                    }
                    if (copiedBuff != null)
                    {
                        foreach (LogMsg msg in copiedBuff)
                        {
                            if (InteractiveMode)
                            {
                                if ((msg & LogMod.INTERACTIVE) != LogMod.NONE)
                                    Console.Write(msg.ToInteractiveString());
                                else if ((msg & LogMod.NORMAL) != LogMod.NONE)
                                    Console.WriteLine(msg.GetContent());
                            }
                            else
                                Console.WriteLine(msg);
                        }
                        copiedBuff.Clear();
                    }
                }
                //--- making sure the buffer is clear on exit
                lock (buffLock)
                {
                    foreach (LogMsg msg in bufferedMsgs)
                    {
                        Console.WriteLine(msg);
                        file.WriteLine(msg);
                    }
                    bufferedMsgs.Clear();
                }
            }
        }
    }
}
