using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.App.Dtos;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;
using TodoList.Utils;

namespace TodoList.App.Tests;

public sealed class TodoItemControllerTestsUpdate : TodoItemControllerTests
{
    [Fact]
    public async Task UpdateTodoItemFully_ReturnsNoContentResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, todoItem.IsDone, TestContext.Current.CancellationToken))
            .ReturnsAsync(EventResult<TodoUpdatedEvent>.Success(new TodoUpdatedEvent(UpdateResult.Updated, todoItem.Id, todoItem.Title, todoItem.IsDone)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title,
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task UpdateTodoItemPartially_ReturnsNoContentResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, null, TestContext.Current.CancellationToken))
            .ReturnsAsync(EventResult<TodoUpdatedEvent>.Success(new TodoUpdatedEvent(UpdateResult.Updated, todoItem.Id, todoItem.Title, null)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            Title = todoItem.Title,
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task UpdateTodoItem_ReturnsBadRequest_WhenTodoItemNotUpdated()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, todoItem.IsDone, TestContext.Current.CancellationToken))
            .ReturnsAsync(EventResult<TodoUpdatedEvent>.Success(new TodoUpdatedEvent(UpdateResult.NotUpdated, todoItem.Id, todoItem.Title, todoItem.IsDone)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title,
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task UpdateTodoItem_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        const string errorMessage = "Failure";
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, todoItem.IsDone, TestContext.Current.CancellationToken))
            .ReturnsAsync(EventResult<TodoUpdatedEvent>.Failure(new ErrorEvent(errorMessage)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title,
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        Logger.ShouldHaveSingleError(errorMessage);
    }
}
