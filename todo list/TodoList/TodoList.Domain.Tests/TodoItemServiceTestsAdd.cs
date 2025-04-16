using Moq;
using TodoList.Domain.Interfaces;

namespace TodoList.Domain.Tests;

public sealed class TodoItemServiceTestsAdd : TodoItemServiceTests
{
    [Fact]
    public async Task AddAsync_ShouldReturnSuccess_WhenTitleIsValid()
    {
        // Arrange
        const string title = "Test Todo";
        const bool isDone = false;
        var todoItem = new TodoItem { Title = title, IsDone = isDone, CreatedAt = DateTime.UtcNow, Id = 1 };
        RepositoryMock.Setup(repo => repo.AddAsync(title, isDone)).ReturnsAsync(todoItem);

        // Act
        var result = await TodoItemService.AddAsync(title, isDone);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(todoItem, result.Value.TodoItem);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Arrange
        const string title = "";
        const bool isDone = false;
        // Act
        var result = await TodoItemService.AddAsync(title, isDone);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Title cannot be empty", result.Error);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnFailure_WhenTitleIsTooLong()
    {
        // Arrange
        var title = new string('a', 101);
        const bool isDone = false;
        // Act
        var result = await TodoItemService.AddAsync(title, isDone);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Title cannot be longer than 100 characters", result.Error);
    }
}