using Moq;
using TodoList.Domain.Services;

namespace TodoList.Domain.Tests;

public class TodoItemServiceTestsAccessAll: TodoItemServiceTests
{
    [Fact]
    public async Task AccessAllAsync_ShouldReturnTodoAccessedAllEvent_WhenCalled()
    {
        // Arrange
        var todoItemService = new TodoItemService(RepositoryMock.Object);
        var expectedItems = new List<TodoItem>
        {
            new TodoItem
            {
                Id = 1,
                CreatedAt = DateTime.UtcNow,
                Title = "Test Todo 1",
                IsDone = false
            },
            new TodoItem
            {
                Id = 2,
                CreatedAt = DateTime.UtcNow,
                Title = "Test Todo 2",
                IsDone = false
            },
        };
        RepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expectedItems);

        // Act
        var result = await todoItemService.AccessAllAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(expectedItems.Count, result.Value.Items.Count);
        foreach (var items in result.Value.Items)
        {
            var expectedItem = expectedItems.FirstOrDefault(i => i.Id == items.Id);
            Assert.NotNull(expectedItem);
        }
    }
}