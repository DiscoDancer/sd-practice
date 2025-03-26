namespace TodoList.App.Dtos;

public sealed class AddInput
{
    public required string Title { get; init; }
    public required bool IsDone { get; init; }
}