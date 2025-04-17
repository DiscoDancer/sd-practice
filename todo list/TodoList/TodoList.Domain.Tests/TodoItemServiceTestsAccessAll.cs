using FluentAssertions;
using Moq;
using TodoList.Domain.Implementations;
using TodoList.Domain.Interfaces;

namespace TodoList.Domain.Tests;

public class TodoItemServiceTestsAccessAll : TodoItemServiceTests
{
    [Fact]
    public async Task AccessAllAsync_ShouldReturnTodoAccessedAllEvent_WhenCalled()
    {
        // Arrange
        var todoItemService = new TodoItemService(RepositoryMock.Object);
        var expectedItems = new List<TodoItem>
        {
            new()
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                Title = "Test Todo 1",
                IsDone = false
            },
            new()
            {
                Id = 2,
                CreatedAt = DateTime.UtcNow,
                Title = "Test Todo 2",
                IsDone = false
            },
        };
        RepositoryMock.Setup(repo => repo.GetAllAsync(TestContext.Current.CancellationToken)).ReturnsAsync(expectedItems);

        // Act
        var result = await todoItemService.AccessAllAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Items.Should().HaveCount(expectedItems.Count);
        result.Value.Items.Should().BeEquivalentTo(expectedItems, options => options.Excluding(item => item.CreatedAt));
    }
}
