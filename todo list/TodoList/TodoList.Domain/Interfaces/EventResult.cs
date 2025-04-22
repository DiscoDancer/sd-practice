using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Interfaces;

public sealed class EventResult<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ErrorEvent Error { get; }
    public T Value { get; }

    public static EventResult<T> Success(T value)
    {
        return new EventResult<T>(true, ErrorEvent.Default, value);
    }

    public static EventResult<T> Failure(ErrorEvent errorEvent)
    {
        return new EventResult<T>(false, errorEvent, default!);
    }

    private EventResult(bool isSuccess, ErrorEvent error, T value)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }
}