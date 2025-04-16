using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Domain;
using TodoList.Domain.Events;

namespace TodoList.App.Tests;

public class TodoItemControllerTestsGetAll : TodoItemControllerTests
{
    [Fact]
    public async Task GetTodoItems_ReturnsOkResult_WithListOfTodoItems()
    {
        // Arrange
        var todoItems = new List<TodoItem>
        {
            new() { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false },
            new() { Id = 2, Title = "SecondItem", CreatedAt = DateTime.UtcNow, IsDone = false }
        };

        var todoAccessedAllEvent = new TodoAccessedAllEvent(todoItems);
        MockService.Setup(x => x.AccessAllAsync()).ReturnsAsync(Result<TodoAccessedAllEvent>.Success(todoAccessedAllEvent));

        // Act
        var result = await Controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<TodoItem>>(okResult.Value);

        Assert.Collection(returnValue,
            item =>
            {
                Assert.Equal(todoItems[0].Id, item.Id);
                Assert.Equal(todoItems[0].Title, item.Title);
                Assert.Equal(todoItems[0].CreatedAt, item.CreatedAt);
                Assert.Equal(todoItems[0].IsDone, item.IsDone);
            },
            item =>
            {
                Assert.Equal(todoItems[1].Id, item.Id);
                Assert.Equal(todoItems[1].Title, item.Title);
                Assert.Equal(todoItems[1].CreatedAt, item.CreatedAt);
                Assert.Equal(todoItems[1].IsDone, item.IsDone);
            });
        Assert.Equal(1, Logger.Collector.Count);
        Assert.Equal(LogLevel.Information, Logger.LatestRecord.Level);
    }
    [Fact]
    public async Task GetTodoItems_ReturnsNotFound_WhenNoTodoItemsExist()
    {
        // Arrange
        MockService.Setup(x => x.AccessAllAsync()).ReturnsAsync(Result<TodoAccessedAllEvent>.Failure("No items found"));

        // Act
        var result = await Controller.GetAll();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("No items found", badRequestResult.Value);
    }

    [Fact]
    public async Task GetTodoItems_ReturnsBadRequest_OnError()
    {
        // Arrange
        var todoAccessedAllEvent = new TodoAccessedAllEvent(new List<TodoItem> { });
        MockService.Setup(x => x.AccessAllAsync()).ReturnsAsync(Result<TodoAccessedAllEvent>.Success(todoAccessedAllEvent));

        // Act
        var result = await Controller.GetAll();

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);

        Assert.Equal(1, Logger.Collector.Count);
        Assert.Equal(LogLevel.Information, Logger.LatestRecord.Level);
    }
}
