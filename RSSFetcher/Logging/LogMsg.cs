namespace RSSFetcher.Logging
{
    /// <summary>
    /// Class representing a message to be logged
    /// </summary>
    /// <param name="severity">The flags that define this msg</param>
    /// <param name="identifier">The reporter who is sending this msg</param>
    /// <param name="content">The content of the msg</param>
    public class LogMsg(LogType type, LogMod mod, string identifier, string content) : LogLevel(type, mod)
    {
        public string IDENTIFIER = identifier;
        public string CONTENT = content;
        public DateTime TIMESTAMP = DateTime.Now;

        public override string ToString()
        {
            return $"[{TIMESTAMP}] {IDENTIFIER}\t{base.ToString()}\t{CONTENT}";
        }
        public string ToInteractiveString()
        {
            if ((_modifier & LogMod.INTERACTIVE) != 0)
                return "> " + CONTENT;
            else
                return CONTENT;
        }
    }
}
