using Moq;

namespace TodoList.Domain.Tests;

public class TodoItemServiceTestsDelete : TodoItemServiceTests
{
    [Fact]
    public async Task DeleteAsync_ValidIdAndDeleted_ReturnsDeletedEvent()
    {
        // Arrange
        const long id = 1;
        RepositoryMock.Setup(repo => repo.DeleteAsync(id)).ReturnsAsync(true);
        // Act
        var result = await TodoItemService.DeleteAsync(id);
        // Assert
        Assert.NotNull(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(DeleteResult.Deleted, result.Value.DeleteResult);
        Assert.Equal(id, result.Value.Id);
    }

    [Fact]
    public async Task DeleteAsync_ValidIdAndNotDeleted_ReturnsDeletedEvent()
    {
        // Arrange
        const long id = 1;
        RepositoryMock.Setup(repo => repo.DeleteAsync(id)).ReturnsAsync(false);
        // Act
        var result = await TodoItemService.DeleteAsync(id);
        // Assert
        Assert.NotNull(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(DeleteResult.NotDeleted, result.Value.DeleteResult);
        Assert.Equal(id, result.Value.Id);
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_ReturnsFailure()
    {
        // Arrange
        const long id = -1;

        // Act
        var result = await TodoItemService.DeleteAsync(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Id must be greater than 0", result.Error);
    }
}