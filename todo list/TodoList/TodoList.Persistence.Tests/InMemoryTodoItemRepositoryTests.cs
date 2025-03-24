using TodoList.Domain;

namespace TodoList.Persistence.Tests;

public class InMemoryTodoItemRepositoryTests
{
    [Fact]
    public async Task GetAsync_WithExistingItem_ReturnsItem()
    {
        // Arrange
        var repository = new InMemoryTodoItemRepository();
        var item = new TodoItem
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            IsDone = false,
            Title = "DoSomething"
        };
        await repository.AddAsync(item);

        // Act
        var result = await repository.GetAsync(1);

        // Assert
        Assert.Equal(item, result);
    }

    [Fact]
    public async Task GetAsync_WithNonExistingItem_ReturnsNull()
    {
        // Arrange
        var repository = new InMemoryTodoItemRepository();

        // Act
        var result = await repository.GetAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_WithNoItems_ReturnsNoItems()
    {
        // Arrange
        var repository = new InMemoryTodoItemRepository();

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithItems_ReturnsItems()
    {
        // Arrange
        var repository = new InMemoryTodoItemRepository();
        var item1 = new TodoItem
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            IsDone = false,
            Title = "DoSomething"
        };
        var item2 = new TodoItem
        {
            Id = 2,
            CreatedAt = DateTime.UtcNow,
            IsDone = false,
            Title = "DoSomethingElse"
        };
        await repository.AddAsync(item1);
        await repository.AddAsync(item2);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Collection(result,
            x => Assert.Equal(item1, x),
            x => Assert.Equal(item2, x));
    }

    [Fact]
    public async Task AddAsync_WithItem_SavesItem()
    {
        // Arrange
        var repository = new InMemoryTodoItemRepository();
        var item = new TodoItem
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            IsDone = false,
            Title = "DoSomething"
        };

        // Act
        await repository.AddAsync(item);
        var result = await repository.GetAsync(1);

        // Assert
        Assert.Equal(item, result);
    }

    [Fact]
    public async Task UpdateAsync_WithExistingItem_UpdatesItem()
    {
        // Arrange
        var repository = new InMemoryTodoItemRepository();
        var item = new TodoItem
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            IsDone = false,
            Title = "DoSomething"
        };
        await repository.AddAsync(item);
        var updatedItem = new TodoItem
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            IsDone = true,
            Title = "DoSomethingElse"
        };

        // Act
        await repository.UpdateAsync(updatedItem);
        var result = await repository.GetAsync(1);

        // Assert
        Assert.Equal(updatedItem, result);
    }

    [Fact]
    public async Task UpdateAsync_WithNonExistingItem_ReturnsFalse()
    {
        // Arrange
        var repository = new InMemoryTodoItemRepository();
        var item = new TodoItem
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            IsDone = false,
            Title = "DoSomething"
        };
        // Act
        var result = await repository.UpdateAsync(item);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingItem_DeletesItem()
    {
        // Arrange
        var repository = new InMemoryTodoItemRepository();
        var item = new TodoItem
        {
            Id = 1,
            CreatedAt = DateTime.UtcNow,
            IsDone = false,
            Title = "DoSomething"
        };
        await repository.AddAsync(item);
        // Act
        await repository.DeleteAsync(1);
        var result = await repository.GetAsync(1);
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingItem_ReturnsFalse()
    {
        // Arrange
        var repository = new InMemoryTodoItemRepository();
        // Act
        var result = await repository.DeleteAsync(1);
        // Assert
        Assert.False(result);
    }
}