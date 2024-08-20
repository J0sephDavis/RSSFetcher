namespace Murong_Xue.Logging
{
    /// <summary>
    /// Class representing a message to be logged
    /// </summary>
    /// <param name="severity">The flags that define this msg</param>
    /// <param name="identifier">The reporter who is sending this msg</param>
    /// <param name="content">The content of the msg</param>
    internal class LogMsg(LogType type, LogMod mod, string identifier, string content) : LogLevel(type, mod)
    {
        public string IDENTIFIER = identifier;
        public string CONTENT = content;
        public string TIMESTAMP = DateTime.Now.ToLongTimeString();

        public override string ToString()
        {
            return $"[{TIMESTAMP}] {IDENTIFIER}\t{base.ToString()}\t{CONTENT}";
        }
        public string GetContent()
        {
            return CONTENT;
        }
        public string ToInteractiveString()
        {
            return "> " + CONTENT;
        }
    }
}
