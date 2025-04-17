using FluentAssertions;
using Moq;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Tests;

public sealed class TodoItemServiceTestsAccess : TodoItemServiceTests
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
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Result.Should().Be(AccessResult.Found);
        result.Value.Id.Should().Be(id);
        result.Value.Item.Should().BeEquivalentTo(todoItem);
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
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Result.Should().Be(AccessResult.NotFound);
        result.Value.Id.Should().Be(id);
        result.Value.Item.Should().BeNull();
    }
}
