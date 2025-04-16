using Moq;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Tests;

public sealed class TodoItemServiceTestsAccess: TodoItemServiceTests
{
    [Fact]
    public async Task AccessAsync_ValidId_ReturnsTodoAccessedEvent()
    {
        // Arrange
        const long id = 1L;
        var todoItem = new TodoItem
        {
            Id = id,
            CreatedAt = DateTime.UtcNow,
            Title = "Test Todo",
            IsDone = false
        };
        RepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync(todoItem);

        // Act
        var result = await TodoItemService.AccessAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(AccessResult.Found, result.Value.Result);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(todoItem, result.Value.Item);
    }

    [Fact]
    public async Task AccessAsync_InvalidId_ReturnsNotFound()
    {
        // Arrange
        const long id = 1L;
        RepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync((TodoItem?)null);
        // Act
        var result = await TodoItemService.AccessAsync(id);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(AccessResult.NotFound, result.Value.Result);
        Assert.Equal(id, result.Value.Id);
        Assert.Null(result.Value.Item);
    }
}