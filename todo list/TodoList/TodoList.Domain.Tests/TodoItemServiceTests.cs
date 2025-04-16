using Moq;
using TodoList.Domain.Implementations;
using TodoList.Domain.Interfaces;

namespace TodoList.Domain.Tests;

public abstract class TodoItemServiceTests
{
    private protected readonly Mock<ITodoItemRepository> RepositoryMock = new();
    private protected ITodoItemService TodoItemService => new TodoItemService(RepositoryMock.Object);
}