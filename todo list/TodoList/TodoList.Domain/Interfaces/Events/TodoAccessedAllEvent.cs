namespace TodoList.Domain.Interfaces.Events;

public sealed class TodoAccessedAllEvent(IReadOnlyCollection<TodoItem> items) : BaseEvent
{
    public IReadOnlyCollection<TodoItem> Items => items;

    protected override IDictionary<string, object?> GetSpecificFields()
    {
        var fields = new Dictionary<string, object?>
        {
            { "Count", items.Count }
        };

        return fields;
    }
}
