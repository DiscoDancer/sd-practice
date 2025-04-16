namespace TodoList.Domain.Events;

public sealed record TodoDeletedEvent(DeleteResult DeleteResult, long Id);