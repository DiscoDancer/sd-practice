namespace TodoList.Domain.Interfaces.Events;

public sealed class TodoDeletedAllEvent(int count) : BaseEvent
{
    public int Count { get; } = count;
    protected override IDictionary<string, object?> GetSpecificFields()
    {
        var fields = new Dictionary<string, object?>
        {
            [nameof(Count)] = Count
        };
        return fields;
    }
}