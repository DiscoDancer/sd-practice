using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;
using TodoList.Utils;

namespace TodoList.App.Tests;

public class TodoItemControllerTestsDeleteAll : TodoItemControllerTests
{
    [Fact]
    public async Task DeleteAllTodoItems_ReturnsNoContentResult_WhenAnyDeleted()
    {
        // Arrange
        MockService.Setup(service => service.DeleteAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(EventResult<TodoDeletedAllEvent>.Success(new TodoDeletedAllEvent(10)));

        // Act
        var result = await Controller.DeleteAll(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsBadRequest_WhenNothingDeleted()
    {
        // Arrange
        MockService.Setup(service => service.DeleteAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(EventResult<TodoDeletedAllEvent>.Success(new TodoDeletedAllEvent(0)));

        // Act
        var result = await Controller.DeleteAll(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestResult>();
        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsBadRequest_WhenServiceFails()
    {
        // Arrange
        const string errorMessage = "Failure";
        MockService.Setup(service => service.DeleteAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(EventResult<TodoDeletedAllEvent>.Failure(errorMessage));

        // Act
        var result = await Controller.DeleteAll(TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        Logger.ShouldHaveSingleError(errorMessage);
    }
}
