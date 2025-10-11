namespace TextAnalytics.Services;

public interface ILoggerService
{
    void LogInfo(string message);
    void LogError(string message);
    void LogWarning(string message);
    void LogSuccess(string message);
}