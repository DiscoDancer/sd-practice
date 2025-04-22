namespace TodoList.Domain.Interfaces;

public sealed class TodoItem
{
    public required long Id { get; init; }
    public required string Title { get; init; }
    public required bool IsDone { get; init; }
    public required DateTime CreatedAt { get; init; }

    public override string ToString()
    {
        return $"Id: {Id}, Title: {Title}, IsDone: {IsDone}, CreatedAt: {CreatedAt:O}";
    }

    public override bool Equals(object? obj)
    {
        return obj is TodoItem other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Title, IsDone, CreatedAt);
    }
    private bool Equals(TodoItem other)
    {
        return Id == other.Id
               && string.Equals(Title, other.Title, StringComparison.Ordinal)
               && IsDone == other.IsDone
               && CreatedAt == other.CreatedAt;
    }

    public static TodoItem Default => new()
    {
        Id = 0,
        Title = "Default",
        IsDone = false,
        CreatedAt = DateTime.MinValue
    };
}
