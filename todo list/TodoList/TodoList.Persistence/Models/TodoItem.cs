using System;
using System.Collections.Generic;

namespace TodoList.Persistence.Models;

public partial class TodoItem
{
    public long Id { get; set; }

    public string Title { get; set; } = null!;

    public bool IsDone { get; set; }

    public DateTime CreatedAt { get; set; }
}
