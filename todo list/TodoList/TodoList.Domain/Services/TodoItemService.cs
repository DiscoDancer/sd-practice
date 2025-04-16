using TodoList.Domain.Events;

namespace TodoList.Domain.Services;

public sealed class TodoItemService(ITodoItemRepository repository) : ITodoItemService
{
    public async Task<Result<TodoCreatedEvent>> AddAsync(string title, bool isDone)
    {
        var titleErrorMessage = CheckTitle(title);
        if (titleErrorMessage is not null)
        {
            return Result<TodoCreatedEvent>.Failure(titleErrorMessage);
        }

        var todoItem = await repository.AddAsync(title, isDone);
        return Result<TodoCreatedEvent>.Success(new TodoCreatedEvent(todoItem));
    }

    public async Task<Result<TodoUpdatedEvent>> UpdateAsync(long id, string? title, bool? isDone)
    {
        var titleErrorMessage = title == null ? title : CheckTitle(title);
        if (titleErrorMessage is not null)
        {
            return Result<TodoUpdatedEvent>.Failure(titleErrorMessage);
        }

        var result = await repository.UpdateAsync(id, title, isDone);

        var status = result ? UpdateResult.Updated : UpdateResult.NotUpdated;
        var eventResult = new TodoUpdatedEvent(status, id, title, isDone);

        return Result<TodoUpdatedEvent>.Success(eventResult);
    }

    public async Task<Result<TodoAccessedEvent>> AccessAsync(long id)
    {
        if (id <= 0)
        {
            return Result<TodoAccessedEvent>.Failure("Id must be greater than 0");
        }

        var result = await repository.GetAsync(id);

        return Result<TodoAccessedEvent>.Success(result == null ? new TodoAccessedEvent(AccessResult.NotFound, id, null) : new TodoAccessedEvent(AccessResult.Found, id, result));
    }

    public async Task<Result<TodoDeletedEvent>> DeleteAsync(long id)
    {
        if (id <= 0)
        {
            return Result<TodoDeletedEvent>.Failure("Id must be greater than 0");
        }

        var deleted = await repository.DeleteAsync(id);
        return deleted
            ? Result<TodoDeletedEvent>.Success(new TodoDeletedEvent(DeleteResult.Deleted, id))
            : Result<TodoDeletedEvent>.Success(new TodoDeletedEvent(DeleteResult.NotDeleted, id));
    }

    public async Task<Result<TodoAccessedAllEvent>> AccessAllAsync()
    {
        var items = await repository.GetAllAsync();

        return Result<TodoAccessedAllEvent>.Success(new TodoAccessedAllEvent(items));
    }


    /// <returns>error message or null</returns>
    private static string? CheckTitleIsNullOrWhiteSpace(string title)
    {
        return string.IsNullOrWhiteSpace(title) ? "Title cannot be empty" : null;
    }

    /// <returns>error message or null</returns>
    private static string? CheckTitleLength(string title)
    {
        return title.Length > 100 ? "Title cannot be longer than 100 characters" : null;
    }

    /// <returns>error message or null</returns>
    private static string? CheckTitle(string title)
    {
        return CheckTitleIsNullOrWhiteSpace(title) ?? CheckTitleLength(title);
    }
}