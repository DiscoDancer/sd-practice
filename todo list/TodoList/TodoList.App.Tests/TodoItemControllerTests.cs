using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoList.App.Controllers;
using TodoList.App.Dtos;
using TodoList.Domain;

namespace TodoList.App.Tests;

public class TodoItemControllerTests
{
    private readonly Mock<ITodoItemRepository> _mockService;
    private readonly TodoItemController _controller;

    public TodoItemControllerTests()
    {
        _mockService = new Mock<ITodoItemRepository>();
        _controller = new TodoItemController(_mockService.Object);
    }

    [Fact]
    public async Task GetTodoItems_ReturnsOkResult_WithListOfTodoItems()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(service => service.GetAllAsync()).ReturnsAsync(new List<TodoItem> { todoItem });

        // Act
        var result = await _controller.GetAllAsync();

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
        _mockService.Setup(service => service.GetAsync(1)).ReturnsAsync(todoItem);

        // Act
        var result = await _controller.GetAsync(1);

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
        _mockService.Setup(service => service.GetAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _controller.GetAsync(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddTodoItem_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(service => service.AddAsync(todoItem.Title, todoItem.IsDone)).ReturnsAsync(todoItem);

        // Act
        var result = await _controller.AddAsync(new AddInput
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
    }

    [Fact]
    public async Task UpdateTodoItem_ReturnsNoContentResult()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(service => service.UpdateAsync(todoItem)).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateAsync(todoItem);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateTodoItem_ReturnsBadRequest_WhenTodoItemNotUpdated()
    {
        // Arrange
        var todoItem = new TodoItem { Id = 1, Title = "FirstItem", CreatedAt = DateTime.UtcNow, IsDone = false };
        _mockService.Setup(service => service.UpdateAsync(todoItem)).ReturnsAsync(false);

        // Act
        var result = await _controller.UpdateAsync(todoItem);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }

    [Fact]
    public async Task DeleteTodoItem_ReturnsNoContentResult()
    {
        // Arrange
        _mockService.Setup(service => service.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteAsync(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteTodoItem_ReturnsBadRequest_WhenTodoItemNotExist()
    {
        // Arrange
        _mockService.Setup(service => service.DeleteAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteAsync(1);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}
