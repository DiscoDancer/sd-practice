namespace TodoList.Domain.Events;

public sealed record TodoUpdatedEvent(UpdateResult UpdateStatus, long Id, string? Title, bool? IsDone);