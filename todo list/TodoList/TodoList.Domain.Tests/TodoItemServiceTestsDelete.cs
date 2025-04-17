using FluentAssertions;
using Moq;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Tests;

public class TodoItemServiceTestsDelete : TodoItemServiceTests
{
    [Fact]
    public async Task DeleteAsync_ValidIdAndDeleted_ReturnsDeletedEvent()
    {
        // Arrange
        const long id = 1;
        RepositoryMock.Setup(repo => repo.DeleteAsync(id, TestContext.Current.CancellationToken)).ReturnsAsync(true);

        // Act
        var result = await TodoItemService.DeleteAsync(id);

        // Assert
        result.Value.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.DeleteResult.Should().Be(DeleteResult.Deleted);
        result.Value.Id.Should().Be(id);
    }

    [Fact]
    public async Task DeleteAsync_ValidIdAndNotDeleted_ReturnsDeletedEvent()
    {
        // Arrange
        const long id = 1;
        RepositoryMock.Setup(repo => repo.DeleteAsync(id, TestContext.Current.CancellationToken)).ReturnsAsync(false);

        // Act
        var result = await TodoItemService.DeleteAsync(id);

        // Assert
        result.Value.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.DeleteResult.Should().Be(DeleteResult.NotDeleted);
        result.Value.Id.Should().Be(id);
    }

    [Fact]
    public async Task DeleteAsync_InvalidId_ReturnsFailure()
    {
        // Arrange
        const long id = -1;

        // Act
        var result = await TodoItemService.DeleteAsync(id);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Id must be greater than 0");
    }
}
