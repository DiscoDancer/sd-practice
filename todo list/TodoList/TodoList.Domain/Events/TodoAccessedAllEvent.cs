namespace TodoList.Domain.Events;

public sealed record TodoAccessedAllEvent(IReadOnlyCollection<TodoItem> Items);