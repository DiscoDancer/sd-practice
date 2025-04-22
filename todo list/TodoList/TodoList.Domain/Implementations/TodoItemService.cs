using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Implementations;

internal sealed class TodoItemService(ITodoItemRepository repository) : ITodoItemService
{
    public async Task<EventResult<TodoCreatedEvent>> AddAsync(string title, bool isDone, CancellationToken cancellationToken)
    {
        var titleErrorMessage = CheckTitle(title);
        if (titleErrorMessage is not null)
        {
            return EventResult<TodoCreatedEvent>.Failure(new ErrorEvent(titleErrorMessage));
        }

        var todoItem = await repository.AddAsync(title, isDone, cancellationToken);
        return EventResult<TodoCreatedEvent>.Success(new TodoCreatedEvent(todoItem));
    }

    public async Task<EventResult<TodoUpdatedEvent>> UpdateAsync(long id, string? title, bool? isDone, CancellationToken cancellationToken)
    {
        var titleErrorMessage = title == null ? title : CheckTitle(title);
        if (titleErrorMessage is not null)
        {
            return EventResult<TodoUpdatedEvent>.Failure(new ErrorEvent(titleErrorMessage));
        }

        var result = await repository.UpdateAsync(id, title, isDone, cancellationToken);

        var status = result ? UpdateResult.Updated : UpdateResult.NotUpdated;
        var eventResult = new TodoUpdatedEvent(status, id, title, isDone);

        return EventResult<TodoUpdatedEvent>.Success(eventResult);
    }

    public async Task<EventResult<TodoAccessedEvent>> AccessAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return EventResult<TodoAccessedEvent>.Failure(new ErrorEvent("Id must be greater than 0"));
        }

        var result = await repository.GetAsync(id, cancellationToken);

        return EventResult<TodoAccessedEvent>.Success(result == null ? new TodoAccessedEvent(AccessResult.NotFound, id, null) : new TodoAccessedEvent(AccessResult.Found, id, result));
    }

    public async Task<EventResult<TodoDeletedEvent>> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return EventResult<TodoDeletedEvent>.Failure(new ErrorEvent("Id must be greater than 0"));
        }

        var deleted = await repository.DeleteAsync(id, cancellationToken);
        return deleted
            ? EventResult<TodoDeletedEvent>.Success(new TodoDeletedEvent(DeleteResult.Deleted, id))
            : EventResult<TodoDeletedEvent>.Success(new TodoDeletedEvent(DeleteResult.NotDeleted, id));
    }

    public async Task<EventResult<TodoDeletedAllEvent>> DeleteAllAsync(CancellationToken cancellationToken)
    {
        var countDeleted = await repository.DeleteAllAsync(cancellationToken);
        return EventResult<TodoDeletedAllEvent>.Success(new TodoDeletedAllEvent(countDeleted));
    }

    public async Task<EventResult<TodoAccessedAllEvent>> AccessAllAsync(CancellationToken cancellationToken)
    {
        var items = await repository.GetAllAsync(cancellationToken);

        return EventResult<TodoAccessedAllEvent>.Success(new TodoAccessedAllEvent(items));
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