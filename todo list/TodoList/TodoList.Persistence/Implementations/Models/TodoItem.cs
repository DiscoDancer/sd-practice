namespace TodoList.Persistence.Implementations.Models;

public partial class TodoItem
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsDone { get; set; }

    public DateTime CreatedAt { get; set; }
}
