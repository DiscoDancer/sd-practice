using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Interfaces;

public interface ITodoItemService
{
    public Task<Result<TodoCreatedEvent>> AddAsync(string title, bool isDone);
    public Task<Result<TodoUpdatedEvent>> UpdateAsync(long id, string? title, bool? isDone);
    public Task<Result<TodoAccessedEvent>> AccessAsync(long id);
    public Task<Result<TodoDeletedEvent>> DeleteAsync(long id);
    public Task<Result<TodoDeletedAllEvent>> DeleteAllAsync();
    public Task<Result<TodoAccessedAllEvent>> AccessAllAsync();
}