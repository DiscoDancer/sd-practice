using System.Diagnostics.Metrics;

namespace TodoList.Persistence.Metrics.SingleActions;

internal abstract class BaseItemAction(Meter meter, string namePrefix, string counterName)
{
    private protected readonly Counter<int> Counter = meter.CreateCounter<int>($"{namePrefix}.{counterName}");

    private protected KeyValuePair<string, object?> CreateProperty(string name, object? value)
    {
        return new KeyValuePair<string, object?>($"{namePrefix}.{name}", value);
    }
}