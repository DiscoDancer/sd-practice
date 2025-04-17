using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.App.Dtos;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.App.Tests;

public sealed class TodoItemControllerTestsAdd : TodoItemControllerTests
{
    [Fact]
    public async Task AddTodoItem_WhenHappyPath_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(service => service.AddAsync(todoItem.Title, todoItem.IsDone))
            .ReturnsAsync(Result<TodoCreatedEvent>.Success(new TodoCreatedEvent(todoItem)));

        // Act
        var result = await Controller.Add(new AddInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        });

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>()
            .Which.Value.Should().BeOfType<TodoItem>()
            .Which.Should().BeEquivalentTo(todoItem, options => options.Excluding(t => t.CreatedAt));

        MockService.Verify(
            service => service.AddAsync(
                It.Is<string>(title => title == todoItem.Title),
                It.Is<bool>(isDone => isDone == todoItem.IsDone)),
            Times.Once);

        Logger.Collector.Count.Should().Be(1);
        Logger.LatestRecord.Level.Should().Be(LogLevel.Information);
    }

    [Fact]
    public async Task AddTodoItem_WhenServiceReturnsError_ReturnsBadRequest()
    {
        // Arrange
        const string errorMessage = "Failure";
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(service => service.AddAsync(todoItem.Title, todoItem.IsDone))
            .ReturnsAsync(Result<TodoCreatedEvent>.Failure(errorMessage));

        // Act
        var result = await Controller.Add(new AddInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        });

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>()
            .Which.Value.Should().Be(errorMessage);
    }
}
