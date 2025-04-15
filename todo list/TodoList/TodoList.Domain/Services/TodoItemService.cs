using TodoList.Domain.Events;

namespace TodoList.Domain.Services;

public sealed class TodoItemService(ITodoItemRepository repository) : ITodoItemService
{
    public async Task<Result<TodoCreatedEvent>> AddAsync(string title, bool isDone)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<TodoCreatedEvent>.Failure("Title cannot be empty");
        }

        if (title.Length > 100)
        {
            return Result<TodoCreatedEvent>.Failure("Title cannot be longer than 100 characters");
        }

        var todoItem = await repository.AddAsync(title, isDone);
        return Result<TodoCreatedEvent>.Success(new TodoCreatedEvent(todoItem));
    }
}