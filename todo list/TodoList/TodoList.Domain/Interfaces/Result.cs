namespace TodoList.Domain.Interfaces;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public T Value { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, null, value);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, error, default!);
    }

    public void Deconstruct(out bool isSuccess, out T? value, out string? error)
    {
        isSuccess = IsSuccess;
        value = Value;
        error = Error;
    }

    private Result(bool isSuccess, string? error, T value)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }
}