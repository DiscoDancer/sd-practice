namespace TodoList.Domain;

public sealed class TodoItem
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required bool IsDone { get; init; }
    public required DateTime CreatedAt { get; init; }

    public override bool Equals(object? obj)
    {
        return obj is TodoItem other && Equals(other);
    }

    private bool Equals(TodoItem other)
    {
        return Id == other.Id && Title == other.Title && IsDone == other.IsDone && CreatedAt == other.CreatedAt;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Title, IsDone, CreatedAt);
    }

    public static bool IsContentEqual(TodoItem a, TodoItem b)
    {
        return a.Title == b.Title && a.IsDone == b.IsDone;
    }
}
