namespace TodoList.Domain.Interfaces.Events;

public sealed class TodoAccessedEvent(AccessResult result, long id, TodoItem item) : BaseEvent
{
    public AccessResult Result => result;
    public long Id => id;
    public TodoItem Item => item;

    public static TodoAccessedEvent NotFound(long id)
    {
        return new TodoAccessedEvent(AccessResult.NotFound, id, TodoItem.Default);
    }

    protected override IDictionary<string, object?> GetSpecificFields()
    {
        var fields = new Dictionary<string, object?>
        {
            [nameof(result)] = result,
            [nameof(TodoItem.Id)] = id,
        };

        if (result != AccessResult.Found)
        {
            return fields;
        }

        fields[nameof(TodoItem.Title)] = item.Title;
        fields[nameof(TodoItem.IsDone)] = item.IsDone;
        fields[nameof(TodoItem.CreatedAt)] = item.CreatedAt;

        return fields;
    }
}