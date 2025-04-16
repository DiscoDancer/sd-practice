using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoList.Domain;
using TodoList.Domain.Events;

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
        MockService.Setup(service => service.DeleteAsync(id))
            .ReturnsAsync(Result<TodoDeletedEvent>.Success(new TodoDeletedEvent(deleteResult, id)));

        // Act
        var result = await Controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.Equal(1, Logger.Collector.Count);
        Assert.Equal(LogLevel.Information, Logger.LatestRecord.Level);
    }


    [Fact]
    public async Task DeleteTodoItem_ReturnsBadRequest_WhenError()
    {
        // Arrange
        const long id = 1;
        const string errorMessage = "FAILURE!!!";
        MockService.Setup(service => service.DeleteAsync(id))
            .ReturnsAsync(Result<TodoDeletedEvent>.Failure(errorMessage));

        // Act
        var result = await Controller.Delete(1);

        // Assert
        var response = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(errorMessage, response.Value);
    }
}