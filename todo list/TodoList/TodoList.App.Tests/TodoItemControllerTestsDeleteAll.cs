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
        MockService.Setup(service => service.DeleteAllAsync()).ReturnsAsync(Result<TodoDeletedAllEvent>.Success(new TodoDeletedAllEvent(10)));

        // Act
        var result = await Controller.DeleteAll();

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(1, Logger.Collector.Count);
        Assert.Equal(LogLevel.Information, Logger.LatestRecord.Level);
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsBadRequest_WhenNothingDeleted()
    {
        // Arrange
        MockService.Setup(service => service.DeleteAllAsync()).ReturnsAsync(Result<TodoDeletedAllEvent>.Success(new TodoDeletedAllEvent(0)));

        // Act
        var result = await Controller.DeleteAll();

        // Assert
        Assert.IsType<BadRequestResult>(result);
        Assert.Equal(1, Logger.Collector.Count);
        Assert.Equal(LogLevel.Information, Logger.LatestRecord.Level);
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        MockService.Setup(service => service.DeleteAllAsync()).ReturnsAsync(Result<TodoDeletedAllEvent>.Failure("Error"));

        // Act
        var result = await Controller.DeleteAll();

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}