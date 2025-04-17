using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.App.Tests;

public class TodoItemControllerTestsDeleteAll : TodoItemControllerTests
{
    [Fact]
    public async Task DeleteAllTodoItems_ReturnsNoContentResult_WhenAnyDeleted()
    {
        // Arrange
        MockService.Setup(service => service.DeleteAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(Result<TodoDeletedAllEvent>.Success(new TodoDeletedAllEvent(10)));

        // Act
        var result = await Controller.DeleteAll(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        Logger.Collector.Count.Should().Be(1);
        Logger.LatestRecord.Level.Should().Be(LogLevel.Information);
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsBadRequest_WhenNothingDeleted()
    {
        // Arrange
        MockService.Setup(service => service.DeleteAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(Result<TodoDeletedAllEvent>.Success(new TodoDeletedAllEvent(0)));

        // Act
        var result = await Controller.DeleteAll(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        Logger.Collector.Count.Should().Be(1);
        Logger.LatestRecord.Level.Should().Be(LogLevel.Information);
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        MockService.Setup(service => service.DeleteAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(Result<TodoDeletedAllEvent>.Failure("Error"));

        // Act
        var result = await Controller.DeleteAll(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
