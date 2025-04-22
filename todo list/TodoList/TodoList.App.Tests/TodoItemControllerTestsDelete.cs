using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;
using TodoList.Utils;

namespace TodoList.App.Tests;

public class TodoItemControllerTestsDelete : TodoItemControllerTests
{
    [Theory]
    [InlineData(DeleteResult.Deleted)]
    [InlineData(DeleteResult.NotDeleted)]
    public async Task DeleteTodoItem_WhenNoError_ReturnsNoContentResult(DeleteResult deleteResult)
    {
        // Arrange
        const long id = 1;
        MockService.Setup(service => service.DeleteAsync(id, TestContext.Current.CancellationToken))
            .ReturnsAsync(EventResult<TodoDeletedEvent>.Success(new TodoDeletedEvent(deleteResult, id)));

        // Act
        var result = await Controller.Delete(1, TestContext.Current.CancellationToken);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        Logger.ShouldHaveSingleInfo();
    }

    [Fact]
    public async Task DeleteTodoItem_ReturnsBadRequest_WhenError()
    {
        // Arrange
        const long id = 1;
        const string errorMessage = "FAILURE!!!";
        MockService.Setup(service => service.DeleteAsync(id, TestContext.Current.CancellationToken))
            .ReturnsAsync(EventResult<TodoDeletedEvent>.Failure(new ErrorEvent(errorMessage)));

        // Act
        var result = await Controller.Delete(1, TestContext.Current.CancellationToken);

        // Assert
        var response = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        response.Value.Should().Be(errorMessage);
        Logger.ShouldHaveSingleError(errorMessage);
    }
}
