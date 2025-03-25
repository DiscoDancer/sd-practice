namespace TodoList.App.Dtos;

public sealed class AddAsyncInput
{
    public required string Title { get; init; }
    public required bool IsDone { get; init; }
}