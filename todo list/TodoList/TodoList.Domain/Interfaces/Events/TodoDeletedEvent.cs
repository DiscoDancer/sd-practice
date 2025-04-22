namespace TodoList.Domain.Interfaces.Events;

public sealed class TodoDeletedEvent(DeleteResult deleteResult, long id) : BaseEvent
{
    public DeleteResult DeleteResult => deleteResult;
    public long Id => id;

    protected override IDictionary<string, object?> GetSpecificFields()
    {
        var fields = new Dictionary<string, object?>
        {
            [nameof(TodoItem.Id)] = id,
            [nameof(Events.DeleteResult)] = deleteResult,
        };

        return fields;
    }
}