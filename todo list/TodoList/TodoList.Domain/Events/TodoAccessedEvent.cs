namespace TodoList.Domain.Events;

public sealed record TodoAccessedEvent(AccessResult Result, long Id, TodoItem? Item);