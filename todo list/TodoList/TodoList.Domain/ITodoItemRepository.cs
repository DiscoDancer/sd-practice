namespace TodoList.Domain;

public interface ITodoItemRepository
{
    Task<TodoItem?> GetAsync(long id);
    Task<IReadOnlyCollection<TodoItem>> GetAllAsync();
    Task<TodoItem> AddAsync(string title, bool isDone);
    Task<bool> UpdateAsync(long id, string? title, bool? isDone);
    Task<bool> DeleteAsync(long id);
    Task<bool> DeleteAllAsync();
}