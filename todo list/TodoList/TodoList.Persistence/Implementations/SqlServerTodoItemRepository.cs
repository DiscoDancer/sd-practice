using Microsoft.EntityFrameworkCore;
using TodoList.Domain.Interfaces;
using TodoList.Persistence.Implementations.Models;
using TodoItem = TodoList.Domain.Interfaces.TodoItem;

namespace TodoList.Persistence.Implementations;

internal sealed class SqlServerTodoItemRepository(MasterContext dbContext) : ITodoItemRepository
{
    public async Task<TodoItem?> GetAsync(long id, CancellationToken cancellationToken = default)
    {
        var todoItem = await FindByIdOrDefaultAsync(id, cancellationToken);
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

    public async Task<IReadOnlyCollection<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var todoItems = await dbContext.TodoItems.ToListAsync(cancellationToken);

        var result = todoItems.Select(todoItem => new TodoItem
        {
            CreatedAt = todoItem.CreatedAt,
            Id = todoItem.Id,
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        }).ToList().AsReadOnly();

        return result;
    }

    public async Task<TodoItem> AddAsync(string title, bool isDone, CancellationToken cancellationToken = default)
    {
        var todoItem = new Models.TodoItem
        {
            Title = title,
            IsDone = isDone,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await dbContext.TodoItems.AddAsync(todoItem, cancellationToken);

        var changes = await dbContext.SaveChangesAsync(cancellationToken);

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

    public async Task<bool> UpdateAsync(long id, string? title, bool? isDone, CancellationToken cancellationToken = default)
    {
        var todoItem = await FindByIdOrDefaultAsync(id, cancellationToken);
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

        var changes = await dbContext.SaveChangesAsync(cancellationToken);
        return changes > 0;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var todoItem = await FindByIdOrDefaultAsync(id, cancellationToken);
        if (todoItem is null)
        {
            return false;
        }

        dbContext.TodoItems.Remove(todoItem);
        var changes = await dbContext.SaveChangesAsync(cancellationToken);

        var hasBeenDeleted = changes > 0;

        return hasBeenDeleted;
    }

    public async Task<int> DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        var todoItems = await dbContext.TodoItems.ToListAsync(cancellationToken);
        if (!todoItems.Any())
        {
            return 0;
        }

        dbContext.TodoItems.RemoveRange(todoItems);
        var changesCount = await dbContext.SaveChangesAsync(cancellationToken);

        return changesCount;
    }

    private async Task<Models.TodoItem?> FindByIdOrDefaultAsync(long id, CancellationToken cancellationToken = default)
    {
        var result = await dbContext.TodoItems.FindAsync([id], cancellationToken: cancellationToken);
        return result ?? null;
    }
}