namespace TodoList.Domain;

public interface ITodoItemRepository
{
    Task<TodoItem?> GetAsync(long id);
    Task<IReadOnlyCollection<TodoItem>> GetAllAsync();
    Task<long?> AddAsync(string title, bool isDone);
    Task<bool> UpdateAsync(TodoItem item);
    Task<bool> DeleteAsync(long id);
}