using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Domain;
using TodoList.Domain.Events;

namespace TodoList.App.Tests;

public sealed class TodoItemControllerTestsGet : TodoItemControllerTests
{
    [Fact]
    public async Task GetTodoItem_ReturnsOkResult_WithTodoItem()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(service => service.AccessAsync(1)).ReturnsAsync(Result<TodoAccessedEvent>.Success(new TodoAccessedEvent(AccessResult.Found, todoItem.Id, todoItem)));

        // Act
        var result = await Controller.Get(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<TodoItem>(okResult.Value);
        Assert.Equal(todoItem.Id, returnValue.Id);
        Assert.Equal(todoItem.Title, returnValue.Title);
        Assert.Equal(todoItem.CreatedAt, returnValue.CreatedAt);
        Assert.Equal(todoItem.IsDone, returnValue.IsDone);
        Assert.Equal(1, Logger.Collector.Count);
        Assert.Equal(LogLevel.Information, Logger.LatestRecord.Level);
    }

    [Fact]
    public async Task GetTodoItem_ReturnsNotFoundResult_WhenTodoItemNotFound()
    {
        // Arrange
        MockService.Setup(service => service.AccessAsync(2)).ReturnsAsync(Result<TodoAccessedEvent>.Success(new TodoAccessedEvent(AccessResult.NotFound, 2, null)));

        // Act
        var result = await Controller.Get(2);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
        Assert.Equal(1, Logger.Collector.Count);
        Assert.Equal(LogLevel.Information, Logger.LatestRecord.Level);
    }

    [Fact]
    public async Task GetTodoItem_ReturnsBadRequest_WhenServiceReturnsError()
    {
        // Arrange
        const string errorMessage = "Failure";
        MockService.Setup(service => service.AccessAsync(1)).ReturnsAsync(Result<TodoAccessedEvent>.Failure(errorMessage));
        // Act
        var result = await Controller.Get(1);
        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
}