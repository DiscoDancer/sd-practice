using System.ComponentModel;
using System.Diagnostics.Metrics;

namespace TodoList.Persistence.Implementations.Metrics.SingleActions;

internal abstract class BaseItemAction(Meter meter, string namePrefix, string counterName)
{
    private protected readonly Counter<int> Counter = meter.CreateCounter<int>($"{namePrefix}.{counterName}");

    private protected KeyValuePair<string, object?> CreateProperty(Properties property, object? value)
    {
        var propertyName = GetEnumDescription(property);
        return new KeyValuePair<string, object?>($"{namePrefix}.{propertyName}", value);
    }

    protected enum Properties
    {
        [Description("id")]
        Id,
        [Description("title")]
        Title,
        [Description("isDone")]
        IsDone,
        [Description("createdAt")]
        CreatedAt,
        [Description("result")]
        Result
    }

    private static string GetEnumDescription(Enum value)
    {
        var fi = value.GetType().GetField(value.ToString());
        if (fi == null)
        {
            throw new ArgumentException($"Field not found for enum value: {value}");
        }
        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }
}
