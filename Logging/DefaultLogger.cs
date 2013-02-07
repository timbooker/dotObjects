using NLog;

namespace dotObjects.Logging
{
    public class DefaultLogger : ILogger
    {
        private Logger Logger { get; set; }

        public DefaultLogger()
        {
            Logger = NLog.LogManager.GetCurrentClassLogger();
        }

        #region ILogger Members

        public virtual void Write(LogType type, string message, params object[] arguments)
        {
            Logger.Log(LogLevel.FromString(type.ToString()), message, arguments);
        }

        #endregion
    }
}
