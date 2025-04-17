using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;

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
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<List<TodoItem>>()
            .Which.Should().BeEquivalentTo(todoItems);

        Logger.Collector.Count.Should().Be(1);
        Logger.LatestRecord.Level.Should().Be(LogLevel.Information);
    }

    [Fact]
    public async Task GetTodoItems_ReturnsNotFound_WhenNoTodoItemsExist()
    {
        // Arrange
        MockService.Setup(x => x.AccessAllAsync()).ReturnsAsync(Result<TodoAccessedAllEvent>.Failure("No items found"));

        // Act
        var result = await Controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be("No items found");
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
        result.Result.Should().BeOfType<NotFoundResult>();

        Logger.Collector.Count.Should().Be(1);
        Logger.LatestRecord.Level.Should().Be(LogLevel.Information);
    }
}
