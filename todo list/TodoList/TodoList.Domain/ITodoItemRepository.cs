namespace TodoList.Domain;

public interface ITodoItemRepository
{
    Task<TodoItem> GetAsync(long id);
    Task<IReadOnlyCollection<TodoItem>> GetAllAsync();
    Task<bool> AddAsync(TodoItem item);
    Task<bool> UpdateAsync(TodoItem item);
    Task<bool> DeleteAsync(long id);
}