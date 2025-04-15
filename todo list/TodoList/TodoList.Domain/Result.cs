namespace TodoList.Domain;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public T? Value { get; }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, null, value);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, error, default);
    }

    private Result(bool isSuccess, string? error, T? value)
    {
        IsSuccess = isSuccess;
        Error = error;
        Value = value;
    }
}