using Microsoft.Extensions.Logging;

namespace TodoList.Domain.Interfaces.Events;

public abstract class BaseEvent
{
    public DateTime EventCreatedAt { get; } = DateTime.UtcNow;

    protected virtual IDictionary<string, object?> GetSpecificFields()
    {
        return new Dictionary<string, object?>();
    }

    public void Log(ILogger logger, LogLevel logLevel)
    {
        var allFields = GetSpecificFields();
        allFields[nameof(EventCreatedAt)] = EventCreatedAt;

        logger.Log(logLevel, "{EventType} {@Event}",
            GetType().Name,
            allFields);
    }

    public void LogError(ILogger logger)
    {
        Log(logger, LogLevel.Error);
    }

    public void LogInformation(ILogger logger)
    {
        Log(logger, LogLevel.Information);
    }
}