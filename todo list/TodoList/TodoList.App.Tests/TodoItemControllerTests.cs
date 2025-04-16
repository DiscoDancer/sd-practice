using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using TodoList.App.Controllers;
using TodoList.App.Dtos;
using TodoList.Domain;
using TodoList.Domain.Events;
using TodoList.Domain.Services;

namespace TodoList.App.Tests;

public class TodoItemControllerTests
{
    private readonly Mock<ITodoItemRepository> _mockRepository = new();
    private readonly Mock<ITodoItemService> _mockService = new();
    private readonly FakeLogger<TodoItemController> _logger = new();

    private TodoItemController Controller => new(_mockRepository.Object, _logger, _mockService.Object);

    [Fact]
    public async Task GetTodoItems_ReturnsOkResult_WithListOfTodoItems()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockRepository.Setup(service => service.GetAllAsync()).ReturnsAsync(new List<TodoItem> { todoItem });

        // Act
        var result = await Controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<TodoItem>>(okResult.Value);
        Assert.Single(returnValue);
        Assert.Equal(todoItem.Id, returnValue[0].Id);
        Assert.Equal(todoItem.Title, returnValue[0].Title);
        Assert.Equal(todoItem.CreatedAt, returnValue[0].CreatedAt);
        Assert.Equal(todoItem.IsDone, returnValue[0].IsDone);
    }

    [Fact]
    public async Task GetTodoItem_ReturnsOkResult_WithTodoItem()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockRepository.Setup(service => service.GetAsync(1)).ReturnsAsync(todoItem);

        // Act
        var result = await Controller.Get(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<TodoItem>(okResult.Value);
        Assert.Equal(todoItem.Id, returnValue.Id);
        Assert.Equal(todoItem.Title, returnValue.Title);
        Assert.Equal(todoItem.CreatedAt, returnValue.CreatedAt);
        Assert.Equal(todoItem.IsDone, returnValue.IsDone);
    }

    [Fact]
    public async Task GetTodoItem_ReturnsNotFoundResult_WhenTodoItemNotFound()
    {
        // Arrange
        _mockRepository.Setup(service => service.GetAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await Controller.Get(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddTodoItem_WhenHappyPath_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(service => service.AddAsync(todoItem.Title, todoItem.IsDone)).ReturnsAsync(Result<TodoCreatedEvent>.Success(new TodoCreatedEvent(todoItem)));

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
        _mockService.Verify(
            service => service.AddAsync(
                It.Is<string>(title => title == todoItem.Title),
                It.Is<bool>(isDone => isDone == todoItem.IsDone)),
            Times.Once);

        Assert.Equal(1, _logger.Collector.Count);
        Assert.Equal(LogLevel.Information, _logger.LatestRecord.Level);
    }

    [Fact]
    public async Task AddTodoItem_WhenServiceReturnsError_ReturnsBadRequest()
    {
        // Arrange
        const string errorMessage = "Failure";
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(service => service.AddAsync(todoItem.Title, todoItem.IsDone)).ReturnsAsync(Result<TodoCreatedEvent>.Failure(errorMessage));

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

    [Fact]
    public async Task UpdateTodoItemFully_ReturnsNoContentResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, todoItem.IsDone))
            .ReturnsAsync(Result<TodoUpdatedEvent>.Success(new TodoUpdatedEvent(UpdateStatus.Updated, todoItem.Id, todoItem.Title, todoItem.IsDone)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title,
        });

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateTodoItemPartially_ReturnsNoContentResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, null))
            .ReturnsAsync(Result<TodoUpdatedEvent>.Success(new TodoUpdatedEvent(UpdateStatus.Updated, todoItem.Id, todoItem.Title, null)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            Title = todoItem.Title,
        });

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateTodoItem_ReturnsBadRequest_WhenTodoItemNotUpdated()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, todoItem.IsDone))
            .ReturnsAsync(Result<TodoUpdatedEvent>.Success(new TodoUpdatedEvent(UpdateStatus.NotUpdated, todoItem.Id, todoItem.Title, todoItem.IsDone)));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title,
        });

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task UpdateTodoItem_ReturnsBadRequest_WhenFailure()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(x => x.UpdateAsync(todoItem.Id, todoItem.Title, todoItem.IsDone))
            .ReturnsAsync(Result<TodoUpdatedEvent>.Failure("Failure!"));

        // Act
        var result = await Controller.Update(todoItem.Id, new UpdateInput
        {
            IsDone = todoItem.IsDone,
            Title = todoItem.Title,
        });

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteTodoItem_ReturnsNoContentResult()
    {
        // Arrange
        _mockRepository.Setup(service => service.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await Controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTodoItem_ReturnsBadRequest_WhenTodoItemNotExist()
    {
        // Arrange
        _mockRepository.Setup(service => service.DeleteAsync(1)).ReturnsAsync(false);

        // Act
        var result = await Controller.Delete(1);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsNoContentResult()
    {
        // Arrange
        _mockRepository.Setup(service => service.DeleteAllAsync()).ReturnsAsync(true);
        // Act
        var result = await Controller.DeleteAll();
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsBadRequest_WhenNoTodoItems()
    {
        // Arrange
        _mockRepository.Setup(service => service.DeleteAllAsync()).ReturnsAsync(false);
        // Act
        var result = await Controller.DeleteAll();
        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}
