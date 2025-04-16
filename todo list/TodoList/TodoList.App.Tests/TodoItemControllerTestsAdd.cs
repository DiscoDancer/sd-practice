using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.App.Dtos;
using TodoList.Domain.Events;
using TodoList.Domain;

namespace TodoList.App.Tests;

public sealed class TodoItemControllerTestsAdd: TodoItemControllerTests
{
    [Fact]
    public async Task AddTodoItem_WhenHappyPath_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(service => service.AddAsync(todoItem.Title, todoItem.IsDone)).ReturnsAsync(Result<TodoCreatedEvent>.Success(new TodoCreatedEvent(todoItem)));

        // Act
        var result = await Controller.Add(new AddInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        });

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnValue = Assert.IsType<TodoItem>(createdAtActionResult.Value);
        Assert.Equal(todoItem.Id, returnValue.Id);
        Assert.Equal(todoItem.Title, returnValue.Title);
        Assert.Equal(todoItem.CreatedAt, returnValue.CreatedAt);
        Assert.Equal(todoItem.IsDone, returnValue.IsDone);
        MockService.Verify(
            service => service.AddAsync(
                It.Is<string>(title => title == todoItem.Title),
                It.Is<bool>(isDone => isDone == todoItem.IsDone)),
            Times.Once);

        Assert.Equal(1, Logger.Collector.Count);
        Assert.Equal(LogLevel.Information, Logger.LatestRecord.Level);
    }

    [Fact]
    public async Task AddTodoItem_WhenServiceReturnsError_ReturnsBadRequest()
    {
        // Arrange
        const string errorMessage = "Failure";
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockService.Setup(service => service.AddAsync(todoItem.Title, todoItem.IsDone)).ReturnsAsync(Result<TodoCreatedEvent>.Failure(errorMessage));

        // Act
        var result = await Controller.Add(new AddInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title
        });

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, badRequestResult.Value);
    }
}