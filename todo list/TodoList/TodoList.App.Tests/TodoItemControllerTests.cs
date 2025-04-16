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

public abstract class TodoItemControllerTests
{
    private protected readonly Mock<ITodoItemRepository> MockRepository = new();
    private protected readonly Mock<ITodoItemService> MockService = new();
    private protected readonly FakeLogger<TodoItemController> Logger = new();

    private protected TodoItemController Controller => new(MockRepository.Object, Logger, MockService.Object);

    [Fact]
    public async Task GetTodoItems_ReturnsOkResult_WithListOfTodoItems()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        MockRepository.Setup(service => service.GetAllAsync()).ReturnsAsync(new List<TodoItem> { todoItem });

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
        MockRepository.Setup(service => service.GetAsync(1)).ReturnsAsync(todoItem);

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
        MockRepository.Setup(service => service.GetAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await Controller.Get(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

   

    [Fact]
    public async Task DeleteTodoItem_ReturnsNoContentResult()
    {
        // Arrange
        MockRepository.Setup(service => service.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await Controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTodoItem_ReturnsBadRequest_WhenTodoItemNotExist()
    {
        // Arrange
        MockRepository.Setup(service => service.DeleteAsync(1)).ReturnsAsync(false);

        // Act
        var result = await Controller.Delete(1);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsNoContentResult()
    {
        // Arrange
        MockRepository.Setup(service => service.DeleteAllAsync()).ReturnsAsync(true);
        // Act
        var result = await Controller.DeleteAll();
        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteAllTodoItems_ReturnsBadRequest_WhenNoTodoItems()
    {
        // Arrange
        MockRepository.Setup(service => service.DeleteAllAsync()).ReturnsAsync(false);
        // Act
        var result = await Controller.DeleteAll();
        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}
