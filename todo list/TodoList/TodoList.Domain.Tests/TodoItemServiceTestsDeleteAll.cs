using Moq;

namespace TodoList.Domain.Tests;

public class TodoItemServiceTestsDeleteAll : TodoItemServiceTests
{
    [Fact]
    public async Task DeleteAllAsync_ShouldReturnSuccess_WhenDeleteAllIsCalled()
    {
        // Arrange
        const int deletedCount = 10;
        RepositoryMock.Setup(repo => repo.DeleteAllAsync()).ReturnsAsync(deletedCount);
        // Act
        var result = await TodoItemService.DeleteAllAsync();

        // Assert
        Assert.NotNull(result.Value);
        Assert.True(result.IsSuccess);
        Assert.Equal(deletedCount, result.Value.Count);
    }
}