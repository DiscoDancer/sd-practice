using Microsoft.EntityFrameworkCore;
using TodoList.Domain;
using TodoList.Persistence.Models;

namespace TodoList.Persistence;

public sealed class SqlServerTodoItemRepository(MasterContext dbContext) : ITodoItemRepository
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

    public async Task<long?> AddAsync(string title, bool isDone)
    {
        var todoItem = new Models.TodoItem
        {
            // sql server should deal with it
            Id = -1,
            Title = title,
            IsDone = isDone,
            CreatedAt = DateTime.UtcNow,
        };

        var result = await dbContext.TodoItems.AddAsync(todoItem);

        var changes = await dbContext.SaveChangesAsync();

        if (changes == 0)
        {
            return null;
        }

        return result.Entity.Id;
    }

    public async Task<bool> UpdateAsync(Domain.TodoItem item)
    {
        var todoItem = await dbContext.TodoItems.FindAsync(item.Id);
        if (todoItem is null)
        {
            return false;
        }

        todoItem.Title = item.Title;
        todoItem.IsDone = item.IsDone;
        todoItem.CreatedAt = item.CreatedAt;

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
}