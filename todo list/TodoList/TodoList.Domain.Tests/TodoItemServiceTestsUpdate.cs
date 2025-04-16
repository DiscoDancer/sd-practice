using Moq;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.Domain.Tests;

public sealed class TodoItemServiceTestsUpdate : TodoItemServiceTests
{
    [Theory]
    [InlineData(true, UpdateResult.Updated)]
    [InlineData(false, UpdateResult.NotUpdated)]
    public async Task UpdateAsync_ShouldReturnSuccess_WhenUpdateIsValid(bool repositoryResult, UpdateResult expectedStatus)
    {
        // Arrange
        const long id = 1;
        const string? title = "Updated Todo";
        bool? isDone = true;
        RepositoryMock.Setup(repo => repo.UpdateAsync(id, title, isDone)).ReturnsAsync(repositoryResult);
        // Act
        var result = await TodoItemService.UpdateAsync(id, title, isDone);
        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expectedStatus, result.Value?.UpdateStatus);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFailure_WhenTitleIsEmpty()
    {
        // Arrange
        const long id = 1;
        const string? title = "";
        bool? isDone = null;
        // Act
        var result = await TodoItemService.UpdateAsync(id, title, isDone);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Title cannot be empty", result.Error);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFailure_WhenTitleIsTooLong()
    {
        // Arrange
        const long id = 1;
        var title = new string('a', 101);
        bool? isDone = null;
        // Act
        var result = await TodoItemService.UpdateAsync(id, title, isDone);
        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Title cannot be longer than 100 characters", result.Error);
    }
}