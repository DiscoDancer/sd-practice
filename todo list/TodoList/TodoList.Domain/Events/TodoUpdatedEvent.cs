namespace TodoList.Domain.Events;

public sealed record TodoUpdatedEvent(UpdateStatus UpdateStatus, long Id, string? Title, bool? IsDone);