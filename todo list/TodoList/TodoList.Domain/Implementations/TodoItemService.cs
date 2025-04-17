using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Implementations;

internal sealed class TodoItemService(ITodoItemRepository repository) : ITodoItemService
{
    public async Task<Result<TodoCreatedEvent>> AddAsync(string title, bool isDone, CancellationToken cancellationToken)
    {
        var titleErrorMessage = CheckTitle(title);
        if (titleErrorMessage is not null)
        {
            return Result<TodoCreatedEvent>.Failure(titleErrorMessage);
        }

        var todoItem = await repository.AddAsync(title, isDone, cancellationToken);
        return Result<TodoCreatedEvent>.Success(new TodoCreatedEvent(todoItem));
    }

    public async Task<Result<TodoUpdatedEvent>> UpdateAsync(long id, string? title, bool? isDone, CancellationToken cancellationToken)
    {
        var titleErrorMessage = title == null ? title : CheckTitle(title);
        if (titleErrorMessage is not null)
        {
            return Result<TodoUpdatedEvent>.Failure(titleErrorMessage);
        }

        var result = await repository.UpdateAsync(id, title, isDone, cancellationToken);

        var status = result ? UpdateResult.Updated : UpdateResult.NotUpdated;
        var eventResult = new TodoUpdatedEvent(status, id, title, isDone);

        return Result<TodoUpdatedEvent>.Success(eventResult);
    }

    public async Task<Result<TodoAccessedEvent>> AccessAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return Result<TodoAccessedEvent>.Failure("Id must be greater than 0");
        }

        var result = await repository.GetAsync(id, cancellationToken);

        return Result<TodoAccessedEvent>.Success(result == null ? new TodoAccessedEvent(AccessResult.NotFound, id, null) : new TodoAccessedEvent(AccessResult.Found, id, result));
    }

    public async Task<Result<TodoDeletedEvent>> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return Result<TodoDeletedEvent>.Failure("Id must be greater than 0");
        }

        var deleted = await repository.DeleteAsync(id, cancellationToken);
        return deleted
            ? Result<TodoDeletedEvent>.Success(new TodoDeletedEvent(DeleteResult.Deleted, id))
            : Result<TodoDeletedEvent>.Success(new TodoDeletedEvent(DeleteResult.NotDeleted, id));
    }

    public async Task<Result<TodoDeletedAllEvent>> DeleteAllAsync(CancellationToken cancellationToken)
    {
        var countDeleted = await repository.DeleteAllAsync(cancellationToken);
        return Result<TodoDeletedAllEvent>.Success(new TodoDeletedAllEvent(countDeleted));
    }

    public async Task<Result<TodoAccessedAllEvent>> AccessAllAsync(CancellationToken cancellationToken)
    {
        var items = await repository.GetAllAsync(cancellationToken);

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