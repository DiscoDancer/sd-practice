using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using TodoList.App.Controllers;
using TodoList.Domain;
using TodoList.Domain.Services;

namespace TodoList.App.Tests;

public abstract class TodoItemControllerTests
{
    private protected readonly Mock<ITodoItemRepository> MockRepository = new();
    private protected readonly Mock<ITodoItemService> MockService = new();
    private protected readonly FakeLogger<TodoItemController> Logger = new();

    private protected TodoItemController Controller => new(MockRepository.Object, Logger, MockService.Object);

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