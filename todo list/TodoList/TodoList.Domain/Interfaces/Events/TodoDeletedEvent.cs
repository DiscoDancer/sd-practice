namespace TodoList.Domain.Interfaces.Events;

public sealed record TodoDeletedEvent(DeleteResult DeleteResult, long Id);