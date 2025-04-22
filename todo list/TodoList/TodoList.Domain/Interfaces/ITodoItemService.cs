using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Interfaces;

public interface ITodoItemService
{
    public Task<EventResult<TodoCreatedEvent>> AddAsync(string title, bool isDone, CancellationToken cancellationToken = default);
    public Task<EventResult<TodoUpdatedEvent>> UpdateAsync(long id, string? title, bool? isDone, CancellationToken cancellationToken = default);
    public Task<EventResult<TodoAccessedEvent>> AccessAsync(long id, CancellationToken cancellationToken = default);
    public Task<EventResult<TodoDeletedEvent>> DeleteAsync(long id, CancellationToken cancellationToken = default);
    public Task<EventResult<TodoDeletedAllEvent>> DeleteAllAsync(CancellationToken cancellationToken = default);
    public Task<EventResult<TodoAccessedAllEvent>> AccessAllAsync(CancellationToken cancellationToken = default);
}