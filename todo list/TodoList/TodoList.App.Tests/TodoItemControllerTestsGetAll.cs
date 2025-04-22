using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;
using TodoList.Utils;

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
        MockService.Setup(x => x.AccessAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(EventResult<TodoAccessedAllEvent>.Success(todoAccessedAllEvent));

        // Act
        var result = await Controller.GetAll(TestContext.Current.CancellationToken);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<List<TodoItem>>()
            .Which.Should().BeEquivalentTo(todoItems);

        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task GetTodoItems_ReturnsNotFound_WhenNoTodoItemsExist()
    {
        // Arrange
        const string errorMessage = "No items found";
        MockService.Setup(x => x.AccessAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(EventResult<TodoAccessedAllEvent>.Failure(errorMessage));

        // Act
        var result = await Controller.GetAll(TestContext.Current.CancellationToken);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be(errorMessage);

        Logger.ShouldHaveSingleError(errorMessage);
    }

    [Fact]
    public async Task GetTodoItems_ReturnsBadRequest_OnError()
    {
        // Arrange
        var todoAccessedAllEvent = new TodoAccessedAllEvent(new List<TodoItem> { });
        MockService.Setup(x => x.AccessAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(EventResult<TodoAccessedAllEvent>.Success(todoAccessedAllEvent));

        // Act
        var result = await Controller.GetAll(TestContext.Current.CancellationToken);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        Logger.ShouldHaveSingleInfo();
    }
}
