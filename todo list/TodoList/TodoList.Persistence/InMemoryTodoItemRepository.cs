using TodoList.Domain;

namespace TodoList.Persistence;

public sealed class InMemoryTodoItemRepository: ITodoItemRepository
{
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

    public Task<bool> AddAsync(TodoItem item)
    {
        _items.Add(item);
        return Task.FromResult(true);
    }

    public async Task<bool> UpdateAsync(TodoItem item)
    {
        var isDeleted = await DeleteAsync(item.Id);
        if (!isDeleted)
        {
            return false;
        }

        return await AddAsync(item);
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