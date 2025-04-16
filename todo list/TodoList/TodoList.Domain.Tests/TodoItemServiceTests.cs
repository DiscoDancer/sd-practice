using Moq;
using TodoList.Domain.Services;

namespace TodoList.Domain.Tests;

public sealed class TodoItemServiceTests
{
    private readonly Mock<ITodoItemRepository> _repositoryMock;
    private readonly ITodoItemService _todoItemService;

    public TodoItemServiceTests()
    {
        _repositoryMock = new Mock<ITodoItemRepository>();
        _todoItemService = new TodoItemService(_repositoryMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnSuccess_WhenTitleIsValid()
    {
        // Arrange
        const string title = "Test Todo";
        const bool isDone = false;
        var todoItem = new TodoItem { Title = title, IsDone = isDone, CreatedAt = DateTime.UtcNow, Id = 1 };
        _repositoryMock.Setup(repo => repo.AddAsync(title, isDone)).ReturnsAsync(todoItem);

        // Act
        var result = await _todoItemService.AddAsync(title, isDone);

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
        var result = await _todoItemService.AddAsync(title, isDone);
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
        var result = await _todoItemService.AddAsync(title, isDone);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Title cannot be longer than 100 characters", result.Error);
    }
}