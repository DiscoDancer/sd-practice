namespace TodoList.Domain.Interfaces.Events;

public sealed class ErrorEvent(string reason) : BaseEvent
{
    public string Reason => reason;

    protected override IDictionary<string, object?> GetSpecificFields()
    {
        var fields = new Dictionary<string, object?>
        {
            ["Reason"] = reason,
        };

        return fields;
    }

    public static ErrorEvent Default => new("NoError");
}