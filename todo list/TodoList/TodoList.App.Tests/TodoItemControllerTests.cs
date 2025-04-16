using Microsoft.Extensions.Logging.Testing;
using Moq;
using TodoList.App.Controllers;
using TodoList.Domain.Services;

namespace TodoList.App.Tests;

public abstract class TodoItemControllerTests
{
    private protected readonly Mock<ITodoItemService> MockService = new();
    private protected readonly FakeLogger<TodoItemController> Logger = new();

    private protected TodoItemController Controller => new(Logger, MockService.Object);
}