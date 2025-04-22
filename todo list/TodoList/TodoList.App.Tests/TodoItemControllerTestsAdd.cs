using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoList.App.Dtos;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;
using TodoList.Utils;

namespace TodoList.App.Tests;

public sealed class TodoItemControllerTestsAdd : TodoItemControllerTests
{
    [Fact]
    public async Task AddTodoItem_WhenHappyPath_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(service => service.AddAsync(todoItem.Title, todoItem.IsDone, TestContext.Current.CancellationToken))
            .ReturnsAsync(EventResult<TodoCreatedEvent>.Success(new TodoCreatedEvent(todoItem)));

        // Act
        var result = await Controller.Add(new AddInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().BeOfType<TodoItem>()
            .Which.Should().BeEquivalentTo(todoItem, options => options.Excluding(t => t.CreatedAt));

        MockService.Verify(
            service => service.AddAsync(
                It.Is<string>(title => title == todoItem.Title),
                It.Is<bool>(isDone => isDone == todoItem.IsDone), TestContext.Current.CancellationToken),
            Times.Once);

        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task AddTodoItem_WhenServiceReturnsError_ReturnsBadRequest()
    {
        // Arrange
        const string errorMessage = "Failure";
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(service => service.AddAsync(todoItem.Title, todoItem.IsDone, TestContext.Current.CancellationToken))
            .ReturnsAsync(EventResult<TodoCreatedEvent>.Failure(new ErrorEvent(errorMessage)));

        // Act
        var result = await Controller.Add(new AddInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var errorEvent = result.As<BadRequestObjectResult>().Value.As<ErrorEvent>();
        errorEvent.Reason.Should().Be(errorMessage);
        Logger.ShouldHaveSingleError(errorMessage);
    }
}
