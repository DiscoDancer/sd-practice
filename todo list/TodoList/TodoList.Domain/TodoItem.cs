namespace TodoList.Domain;

public sealed class TodoItem
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required bool IsDone { get; init; }
    public required DateTime CreatedAt { get; init; }


    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, IsDone: {IsDone}, CreatedAt: {CreatedAt}";
    }
}
