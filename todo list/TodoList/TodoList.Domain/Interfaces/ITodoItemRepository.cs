namespace TodoList.Domain.Interfaces;

public interface ITodoItemRepository
{
    Task<TodoItem?> GetAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TodoItem> AddAsync(string title, bool isDone, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(long id, string? title, bool? isDone, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
    Task<int> DeleteAllAsync(CancellationToken cancellationToken = default);
}