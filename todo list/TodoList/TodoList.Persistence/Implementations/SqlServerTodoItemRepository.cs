using Microsoft.EntityFrameworkCore;
using TodoList.Domain;
using TodoList.Domain.Metrics;
using TodoList.Persistence.Implementations.Models;

namespace TodoList.Persistence.Implementations;

internal sealed class SqlServerTodoItemRepository(MasterContext dbContext, ITodoItemMetrics todoItemMetrics) : ITodoItemRepository
{
    public async Task<Domain.TodoItem?> GetAsync(long id)
    {
        var todoItem = await FindByIdOrDefaultAsync(id);
        if (todoItem is null)
        {
            return null;
        }


        var result = new Domain.TodoItem
        {
            CreatedAt = todoItem.CreatedAt,
            Id = todoItem.Id,
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        };

        todoItemMetrics.ItemRetrieved(result);

        return result;
    }

    public async Task<IReadOnlyCollection<Domain.TodoItem>> GetAllAsync()
    {
        var todoItems = await dbContext.TodoItems.ToListAsync();

        var result = todoItems.Select(todoItem => new Domain.TodoItem
        {
            CreatedAt = todoItem.CreatedAt,
            Id = todoItem.Id,
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        }).ToList().AsReadOnly();

        todoItemMetrics.ItemsRetrieved(todoItems.Count);

        return result;
    }

    public async Task<Domain.TodoItem> AddAsync(string title, bool isDone)
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

        var domainObject = new Domain.TodoItem
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
        if (changes > 0)
        {
            todoItemMetrics.ItemUpdated(id, title, isDone);
            return true;
        }

        return false;
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
        if (hasBeenDeleted)
        {
            todoItemMetrics.ItemDeleted(id);
        }

        return hasBeenDeleted;
    }

    public async Task<bool> DeleteAllAsync()
    {
        var todoItems = await dbContext.TodoItems.ToListAsync();
        if (!todoItems.Any())
        {
            return false;
        }

        dbContext.TodoItems.RemoveRange(todoItems);
        var changesCount = await dbContext.SaveChangesAsync();

        if (changesCount > 0)
        {
            todoItemMetrics.ItemsDeleted(changesCount);
            return true;
        }

        return false;
    }

    private async Task<Models.TodoItem?> FindByIdOrDefaultAsync(long id)
    {
        var result = await dbContext.TodoItems.FindAsync(id);
        if (result is null)
        {
            todoItemMetrics.ItemSearchedById(id, false);
            return null;
        }

        todoItemMetrics.ItemSearchedById(id, true);
        return result;
    }
}