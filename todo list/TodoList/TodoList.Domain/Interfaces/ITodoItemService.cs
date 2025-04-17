using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Interfaces;

public interface ITodoItemService
{
    public Task<Result<TodoCreatedEvent>> AddAsync(string title, bool isDone, CancellationToken cancellationToken = default);
    public Task<Result<TodoUpdatedEvent>> UpdateAsync(long id, string? title, bool? isDone, CancellationToken cancellationToken = default);
    public Task<Result<TodoAccessedEvent>> AccessAsync(long id, CancellationToken cancellationToken = default);
    public Task<Result<TodoDeletedEvent>> DeleteAsync(long id, CancellationToken cancellationToken = default);
    public Task<Result<TodoDeletedAllEvent>> DeleteAllAsync(CancellationToken cancellationToken = default);
    public Task<Result<TodoAccessedAllEvent>> AccessAllAsync(CancellationToken cancellationToken = default);
}