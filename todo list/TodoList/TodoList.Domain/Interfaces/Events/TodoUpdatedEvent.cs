namespace TodoList.Domain.Interfaces.Events;

public sealed class TodoUpdatedEvent(UpdateResult updateStatus, long id, string? title, bool? isDone) : BaseEvent
{
    public UpdateResult UpdateStatus => updateStatus;
    public long Id => id;
    public string? Title => title;
    public bool? IsDone => isDone;

    protected override IDictionary<string, object?> GetSpecificFields()
    {
        var fields = new Dictionary<string, object?>
        {
            [nameof(TodoItem.Id)] = id,
            [nameof(TodoItem.Title)] = title,
            [nameof(TodoItem.IsDone)] = isDone,
            [nameof(UpdateResult)] = updateStatus
        };

        return fields;
    }
}