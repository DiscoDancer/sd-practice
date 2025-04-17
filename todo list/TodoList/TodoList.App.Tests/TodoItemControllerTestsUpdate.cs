using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.App.Dtos;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.App.Tests;

public sealed class TodoItemControllerTestsUpdate : TodoItemControllerTests
{
    [Fact]
    public async Task UpdateTodoItemFully_ReturnsNoContentResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, todoItem.IsDone, TestContext.Current.CancellationToken))
            .ReturnsAsync(Result<TodoUpdatedEvent>.Success(new TodoUpdatedEvent(UpdateResult.Updated, todoItem.Id, todoItem.Title, todoItem.IsDone)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title,
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        Logger.Collector.Count.Should().Be(1);
        Logger.LatestRecord.Level.Should().Be(LogLevel.Information);
    }

    [Fact]
    public async Task UpdateTodoItemPartially_ReturnsNoContentResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, null, TestContext.Current.CancellationToken))
            .ReturnsAsync(Result<TodoUpdatedEvent>.Success(new TodoUpdatedEvent(UpdateResult.Updated, todoItem.Id, todoItem.Title, null)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            Title = todoItem.Title,
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        Logger.Collector.Count.Should().Be(1);
        Logger.LatestRecord.Level.Should().Be(LogLevel.Information);
    }

    [Fact]
    public async Task UpdateTodoItem_ReturnsBadRequest_WhenTodoItemNotUpdated()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, todoItem.IsDone, TestContext.Current.CancellationToken))
            .ReturnsAsync(Result<TodoUpdatedEvent>.Success(new TodoUpdatedEvent(UpdateResult.NotUpdated, todoItem.Id, todoItem.Title, todoItem.IsDone)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title,
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        Logger.Collector.Count.Should().Be(1);
        Logger.LatestRecord.Level.Should().Be(LogLevel.Information);
    }

    [Fact]
    public async Task UpdateTodoItem_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, todoItem.IsDone, TestContext.Current.CancellationToken))
            .ReturnsAsync(Result<TodoUpdatedEvent>.Failure("Failure!"));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title,
        }, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
