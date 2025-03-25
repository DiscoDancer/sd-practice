using TodoList.Domain;

namespace TodoList.Persistence;

public sealed class InMemoryTodoItemRepository : ITodoItemRepository
{
    private static long _id = 1;

    private readonly List<TodoItem> _items = [];

    public Task<TodoItem?> GetAsync(long id)
    {
        return Task.FromResult(_items.FirstOrDefault(x => x.Id == id));
    }

    public Task<IReadOnlyCollection<TodoItem>> GetAllAsync()
    {
        IReadOnlyCollection<TodoItem> readOnlyItems = _items.AsReadOnly();
        return Task.FromResult(readOnlyItems);
    }

    public Task<long?> AddAsync(string title, bool isDone)
    {
        _items.Add(new TodoItem
        {
            CreatedAt = DateTime.UtcNow,
            Title = title,
            IsDone = isDone,
            Id = ++_id,
        });

        return Task.FromResult((long?)_id);
    }

    public async Task<bool> UpdateAsync(TodoItem item)
    {
        var isDeleted = await DeleteAsync(item.Id);
        if (!isDeleted)
        {
            return false;
        }

        _items.Add(new TodoItem
        {
            CreatedAt = DateTime.UtcNow,
            Title = item.Title,
            IsDone = item.IsDone,
            Id = item.Id,
        });

        return true;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var foundItem = await GetAsync(id);
        if (foundItem is null)
        {
            return false;
        }

        _items.Remove(foundItem);

        return true;
    }
}