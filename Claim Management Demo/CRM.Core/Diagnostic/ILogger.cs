namespace CRM.Core.Diagnostic
{
    public interface ILogger
    {
        void Fatal(string message, params object[] args);
        void Error(string message, params object[] args);
        void Warning(string message, params object[] args);
        void Debug(string message, params object[] args);
        void Info(string message, params object[] args);
    }
}
