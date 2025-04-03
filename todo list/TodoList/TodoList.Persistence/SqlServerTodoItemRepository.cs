using Microsoft.EntityFrameworkCore;
using TodoList.Domain;
using TodoList.Persistence.Models;

namespace TodoList.Persistence;

public sealed class SqlServerTodoItemRepository(MasterContext dbContext, ITodoItemMetrics todoItemMetrics) : ITodoItemRepository
{
    public async Task<Domain.TodoItem?> GetAsync(long id)
    {
        var todoItem = await dbContext.TodoItems.FindAsync(id);
        if (todoItem is null)
        {
            return null;
        }

        return new Domain.TodoItem
        {
            CreatedAt = todoItem.CreatedAt,
            Id = todoItem.Id,
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        };
    }

    public async Task<IReadOnlyCollection<Domain.TodoItem>> GetAllAsync()
    {
        var todoItems = await dbContext.TodoItems.ToListAsync();
        return todoItems.Select(todoItem => new Domain.TodoItem
        {
            CreatedAt = todoItem.CreatedAt,
            Id = todoItem.Id,
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        }).ToList().AsReadOnly();
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
        todoItemMetrics.ItemCreated(domainObject);

        return domainObject;
    }

    public async Task<bool> UpdateAsync(long id, string? title, bool? isDone)
    {
        var todoItem = await dbContext.TodoItems.FindAsync(id);
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
        var todoItem = await dbContext.TodoItems.FindAsync(id);
        if (todoItem is null)
        {
            return false;
        }

        dbContext.TodoItems.Remove(todoItem);
        var changes = await dbContext.SaveChangesAsync();

        return changes > 0;
    }

    public async Task<bool> DeleteAllAsync()
    {
        var todoItems = await dbContext.TodoItems.ToListAsync();
        if (!todoItems.Any())
        {
            return false;
        }

        dbContext.TodoItems.RemoveRange(todoItems);
        var changes = await dbContext.SaveChangesAsync();

        return changes > 0;
    }
}