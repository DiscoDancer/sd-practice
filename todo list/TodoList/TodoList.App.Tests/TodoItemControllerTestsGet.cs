using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;
using TodoList.Utils;

namespace TodoList.App.Tests;

public sealed class TodoItemControllerTestsGet : TodoItemControllerTests
{
    [Fact]
    public async Task GetTodoItem_ReturnsOkResult_WithTodoItem()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(service => service.AccessAsync(1, TestContext.Current.CancellationToken)).ReturnsAsync(EventResult<TodoAccessedEvent>.Success(new TodoAccessedEvent(AccessResult.Found, todoItem.Id, todoItem)));

        // Act
        var result = await Controller.Get(1, TestContext.Current.CancellationToken);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeOfType<TodoItem>()
            .Which.Should().BeEquivalentTo(todoItem);

        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task GetTodoItem_ReturnsNotFoundResult_WhenTodoItemNotFound()
    {
        // Arrange
        MockService.Setup(service => service.AccessAsync(2, TestContext.Current.CancellationToken)).ReturnsAsync(EventResult<TodoAccessedEvent>.Success(new TodoAccessedEvent(AccessResult.NotFound, 2, null)));

        // Act
        var result = await Controller.Get(2, TestContext.Current.CancellationToken);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();

        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task GetTodoItem_ReturnsBadRequest_WhenServiceReturnsError()
    {
        // Arrange
        const string errorMessage = "Failure";
        MockService.Setup(service => service.AccessAsync(1, TestContext.Current.CancellationToken)).ReturnsAsync(EventResult<TodoAccessedEvent>.Failure(errorMessage));

        // Act
        var result = await Controller.Get(1, TestContext.Current.CancellationToken);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be(errorMessage);

        Logger.ShouldHaveSingleError(errorMessage);
    }
}
