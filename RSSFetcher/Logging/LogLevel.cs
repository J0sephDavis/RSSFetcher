namespace RSSFetcher.Logging
{
    [Flags]
    public enum LogMod
    {
        NONE = 0,
        NORMAL = 1 << 0,
        SPAM = 1 << 1,
        VERBOSE = 1 << 2,
        UNIMPORTANT = 1 << 3,
        INTERACTIVE = 1 << 4,

        ALL = SPAM | VERBOSE | UNIMPORTANT | NORMAL | INTERACTIVE,
#if DEBUG
        _DEFAULT = NORMAL | INTERACTIVE,
        DEFAULT = _DEFAULT | UNIMPORTANT,
#else
        DEFAULT = NORMAL | INTERACTIVE,
#endif
    }
    [Flags]
    public enum LogType
    {
        NONE = 0,
        DEBUG = 1 << 0,
        ERROR = 1 << 1,
        OUTPUT = 1 << 2,

        ALL = DEBUG | ERROR | OUTPUT,
#if DEBUG
        _DEFAULT = OUTPUT | ERROR,
        DEFAULT = _DEFAULT | DEBUG
#else
        DEFAULT = OUTPUT | ERROR
#endif
    }
    public class LogLevel(LogType type, LogMod mod)
    {
        protected LogMod _modifier = mod;
        protected LogType _type = type;
        //------ getters/setters
        public LogMod GetLMod()
        {
            return _modifier;
        }
        public LogType GetLType()
        {
            return _type;
        }
        //------ string formatting
        protected static string Mod_ToString(LogMod mod)
        {
            string test = string.Empty;
            test += (mod & LogMod.NORMAL) > 0 ? "N" : "_";
            test += (mod & LogMod.SPAM) > 0 ? "S" : "_";
            test += (mod & LogMod.VERBOSE) > 0 ? "V" : "_";
            test += (mod & LogMod.UNIMPORTANT) > 0 ? "U" : "_";
            test += (mod & LogMod.INTERACTIVE) > 0 ? "I" : "_";
            return test;
        }
        override public string ToString()
        {
            return $"[{_type}({Mod_ToString(_modifier)})]";
        }
        //------ Set
        public void Set(LogMod mod)
        {
            _modifier = mod;
        }
        public void Set(LogType type)
        {
            _type = type;
        }
        public void Set(LogLevel level)
        {
            _type = level._type;
            _modifier = level._modifier;
        }
        //----- Mask
        public void Mask(LogMod mod)
        {
            _modifier |= mod;
        }
        public void Mask(LogType type)
        {
            _type |= type;
        }
        public void Mask(LogLevel level)
        {
            _type |= level._type;
            _modifier |= level._modifier;
        }
        //---- bitewise AND
        public static LogMod operator &(LogLevel a, LogMod b)
        {
            return a._modifier & b;
        }
        public static LogType operator &(LogLevel a, LogType b)
        {
            return a._type & b;
        }
        //---- bitewise OR
        public static LogLevel operator |(LogLevel a, LogLevel b)
        {
            return new LogLevel(
                a._type | b._type,
                a._modifier | b._modifier);
        }
        public static LogLevel operator |(LogLevel a, LogMod b)
        {
            LogLevel tmp = new(a._type, a._modifier);
            tmp.Mask(b);
            return tmp;
        }
        public static LogLevel operator |(LogLevel a, LogType b)
        {
            LogLevel tmp = new(a._type, a._modifier);
            tmp.Mask(b);
            return tmp;
        }
        //---- comparisons
        // We double up on these because implicit conversions
        // from LogMsg to LogLevel will be made
        // if there isn't a matching signature:
        // 1. msg == level
        public static bool operator ==(LogLevel a, LogMsg m)
        {
            return (a._modifier & m._modifier) == m._modifier
                && (a._type & m._type) == m._type;
        }
        public static bool operator !=(LogLevel a, LogMsg m)
        {

            return (a._modifier & m._modifier) != m._modifier
                && (a._type & m._type) != m._type;
        }
        // 2. message == level
        public static bool operator ==(LogMsg m, LogLevel a)
        {
            return (a._modifier & m._modifier) == m._modifier
                && (a._type & m._type) == m._type;
        }
        public static bool operator !=(LogMsg m, LogLevel a)
        {
            return (a._modifier & m._modifier) != m._modifier
                && (a._type & m._type) != m._type;
        }
        //---- comparisons to base types
        public static bool operator ==(LogLevel a, LogMod m)
        {
            return a._modifier == m;
        }
        public static bool operator !=(LogLevel a, LogMod m)
        {
            return a._modifier != m;
        }
        public static bool operator ==(LogLevel a, LogType t)
        {
            return a._type == t;
        }
        public static bool operator !=(LogLevel a, LogType t)
        {
            return a._type != t;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            return Equals((LogLevel)obj);
        }
        public bool Equals(LogLevel obj)
        {
            return (
                (GetLMod() == obj.GetLMod())
                && (GetLType() == obj.GetLType())
            );
        }
    }
}
