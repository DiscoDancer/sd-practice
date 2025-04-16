using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Interfaces;
using TodoList.Persistence.Implementations.Models;
using TodoItem = TodoList.Domain.Interfaces.TodoItem;

namespace TodoList.Persistence.Implementations;

internal sealed class SqlServerTodoItemRepository(MasterContext dbContext) : ITodoItemRepository
{
    public async Task<Domain.Interfaces.TodoItem?> GetAsync(long id)
    {
        var todoItem = await FindByIdOrDefaultAsync(id);
        if (todoItem is null)
        {
            return null;
        }

        var result = new TodoItem
        {
            CreatedAt = todoItem.CreatedAt,
            Id = todoItem.Id,
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        };

        return result;
    }

    public async Task<IReadOnlyCollection<Domain.Interfaces.TodoItem>> GetAllAsync()
    {
        var todoItems = await dbContext.TodoItems.ToListAsync();

        var result = todoItems.Select(todoItem => new TodoItem
        {
            CreatedAt = todoItem.CreatedAt,
            Id = todoItem.Id,
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        }).ToList().AsReadOnly();

        return result;
    }

    public async Task<Domain.Interfaces.TodoItem> AddAsync(string title, bool isDone)
    {
        var todoItem = new Models.TodoItem
        {
            Title = title,
            IsDone = isDone,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await dbContext.TodoItems.AddAsync(todoItem);

        var changes = await dbContext.SaveChangesAsync();

        if (changes == 0)
        {
            throw new Exception("Failed to save todo item");
        }

        var domainObject = new TodoItem
        {
            CreatedAt = result.Entity.CreatedAt,
            Id = result.Entity.Id,
            IsDone = result.Entity.IsDone,
            Title = result.Entity.Title
        };

        return domainObject;
    }

    public async Task<bool> UpdateAsync(long id, string? title, bool? isDone)
    {
        var todoItem = await FindByIdOrDefaultAsync(id);
        if (todoItem is null)
        {
            return false;
        }

        if (title is not null)
        {
            todoItem.Title = title;
        }
        if (isDone is not null)
        {
            todoItem.IsDone = isDone.Value;
        }

        var changes = await dbContext.SaveChangesAsync();
        return changes > 0;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var todoItem = await FindByIdOrDefaultAsync(id);
        if (todoItem is null)
        {
            return false;
        }

        dbContext.TodoItems.Remove(todoItem);
        var changes = await dbContext.SaveChangesAsync();

        var hasBeenDeleted = changes > 0;

        return hasBeenDeleted;
    }

    public async Task<int> DeleteAllAsync()
    {
        var todoItems = await dbContext.TodoItems.ToListAsync();
        if (!todoItems.Any())
        {
            return 0;
        }

        dbContext.TodoItems.RemoveRange(todoItems);
        var changesCount = await dbContext.SaveChangesAsync();

        return changesCount;
    }

    private async Task<Models.TodoItem?> FindByIdOrDefaultAsync(long id)
    {
        var result = await dbContext.TodoItems.FindAsync(id);
        return result ?? null;
    }
}