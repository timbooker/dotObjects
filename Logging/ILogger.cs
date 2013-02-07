namespace dotObjects.Logging
{
    public interface ILogger
    {
        void Write(LogType type, string message, params object[] arguments);
    }
}
