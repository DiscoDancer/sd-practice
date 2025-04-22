using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Interfaces;

public sealed class EventResult<T> where T : BaseEvent
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public ErrorEvent Error { get; }
    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException("Cannot access Value when result is failure.");
            return _value!;
        }
    }

    public static EventResult<T> Success(T value)
    {
        return new EventResult<T>(true, ErrorEvent.Default, value);
    }

    public static EventResult<T> Failure(ErrorEvent errorEvent)
    {
        return new EventResult<T>(false, errorEvent, null!);
    }

    private EventResult(bool isSuccess, ErrorEvent error, T value)
    {
        IsSuccess = isSuccess;
        Error = error;
        _value = value;
    }

    private readonly T? _value;
}