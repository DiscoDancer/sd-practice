using Moq;
using TodoList.Domain.Services;

namespace TodoList.Domain.Tests;

public abstract class TodoItemServiceTests
{
    private protected readonly Mock<ITodoItemRepository> RepositoryMock = new();
    private protected ITodoItemService TodoItemService => new TodoItemService(RepositoryMock.Object);
}