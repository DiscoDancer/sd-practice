using TodoList.Domain.Interfaces;

namespace TodoList.Domain.Interfaces.Events;

public sealed record TodoAccessedAllEvent(IReadOnlyCollection<TodoItem> Items);