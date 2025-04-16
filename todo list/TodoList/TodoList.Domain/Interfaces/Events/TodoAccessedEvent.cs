using TodoList.Domain.Interfaces;

namespace TodoList.Domain.Interfaces.Events;

public sealed record TodoAccessedEvent(AccessResult Result, long Id, TodoItem? Item);