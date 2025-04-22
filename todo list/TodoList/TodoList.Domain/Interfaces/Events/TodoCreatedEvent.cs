namespace TodoList.Domain.Interfaces.Events;

public sealed class TodoCreatedEvent(TodoItem todoItem) : BaseEvent
{
    public TodoItem TodoItem => todoItem;

    protected override IDictionary<string, object?> GetSpecificFields()
    {
        var fields = new Dictionary<string, object?>
        {
            [nameof(TodoItem.Id)] = todoItem.Id,
            [nameof(TodoItem.Title)] = todoItem.Title,
            [nameof(TodoItem.IsDone)] = todoItem.IsDone,
            [nameof(TodoItem.CreatedAt)] = todoItem.CreatedAt
        };

        return fields;
    }
}