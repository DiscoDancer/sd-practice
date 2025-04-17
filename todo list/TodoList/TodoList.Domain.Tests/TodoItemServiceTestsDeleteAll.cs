using FluentAssertions;
using Moq;

namespace TodoList.Domain.Tests;

public class TodoItemServiceTestsDeleteAll : TodoItemServiceTests
{
    [Fact]
    public async Task DeleteAllAsync_ShouldReturnSuccess_WhenDeleteAllIsCalled()
    {
        // Arrange
        const int deletedCount = 10;
        RepositoryMock.Setup(repo => repo.DeleteAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(deletedCount);

        // Act
        var result = await TodoItemService.DeleteAllAsync();

        // Assert
        result.Value.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Count.Should().Be(deletedCount);
    }
}
