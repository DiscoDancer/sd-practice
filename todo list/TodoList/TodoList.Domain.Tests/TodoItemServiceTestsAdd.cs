using FluentAssertions;
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
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.TodoItem.Should().BeEquivalentTo(todoItem);
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
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Title cannot be empty");
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
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Title cannot be longer than 100 characters");
    }
}
