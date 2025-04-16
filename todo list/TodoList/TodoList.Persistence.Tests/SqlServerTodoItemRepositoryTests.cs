using Microsoft.EntityFrameworkCore;
using TodoList.Persistence.Implementations;
using TodoList.Persistence.Implementations.Models;
using TodoItem = TodoList.Persistence.Implementations.Models.TodoItem;

namespace TodoList.Persistence.Tests;

public class SqlServerTodoItemRepositoryTests
{

    private static DbContextOptions<MasterContext> GetInMemoryOptions()
    {
        return new DbContextOptionsBuilder<MasterContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
            .Options;
    }

    [Fact]
    public async Task AddAsync_ShouldAddTodoItem()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext);

        // Act
        var todoItem = await repository.AddAsync("Test", false);

        // Assert
        Assert.NotNull(todoItem);
        Assert.NotEqual(0, todoItem.Id);
        Assert.Equal("Test", todoItem.Title);
        Assert.False(todoItem.IsDone);

        var savedTodoItem = await dbContext.TodoItems.FindAsync([todoItem.Id], TestContext.Current.CancellationToken);
        Assert.NotNull(savedTodoItem);
        Assert.Equal(todoItem.Id, savedTodoItem.Id);
        Assert.Equal(todoItem.Title, savedTodoItem.Title);
        Assert.Equal(todoItem.IsDone, savedTodoItem.IsDone);
        Assert.Equal(todoItem.CreatedAt, savedTodoItem.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTodoItem()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext);

        var todoItem = dbContext.TodoItems.Add(new TodoItem
        {
            Title = "Initial Title",
            IsDone = false,
            CreatedAt = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var updatedTodoItem = new TodoItem
        {
            Id = todoItem.Entity.Id,
            Title = "Updated Title",
            IsDone = true,
            CreatedAt = todoItem.Entity.CreatedAt
        };
        await repository.UpdateAsync(todoItem.Entity.Id, updatedTodoItem.Title, updatedTodoItem.IsDone);

        // Assert
        var retrievedTodoItem = await dbContext.TodoItems.FindAsync([todoItem.Entity.Id], TestContext.Current.CancellationToken);
        Assert.NotNull(retrievedTodoItem);
        Assert.Equal(updatedTodoItem.Id, retrievedTodoItem.Id);
        Assert.Equal(updatedTodoItem.Title, retrievedTodoItem.Title);
        Assert.Equal(updatedTodoItem.IsDone, retrievedTodoItem.IsDone);
        Assert.Equal(updatedTodoItem.CreatedAt, retrievedTodoItem.CreatedAt);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateReturnFalse_WhenItemNotFoundById()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext);
        // Act
        var result = await repository.UpdateAsync(999, "Updated Title", true);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoItem()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext);

        var todoItem = dbContext.TodoItems.Add(new TodoItem
        {
            Title = "Test",
            IsDone = false,
            CreatedAt = DateTime.UtcNow,
        });

        // Act
        var retrievedTodoItem = await repository.GetAsync(todoItem.Entity.Id);

        // Assert
        Assert.NotNull(retrievedTodoItem);
        Assert.Equal(todoItem.Entity.Id, retrievedTodoItem.Id);
        Assert.Equal(todoItem.Entity.Title, retrievedTodoItem.Title);
        Assert.Equal(todoItem.Entity.IsDone, retrievedTodoItem.IsDone);
        Assert.Equal(todoItem.Entity.CreatedAt, retrievedTodoItem.CreatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext);

        var todoItem = dbContext.TodoItems.Add(new TodoItem
        {
            Title = "Test",
            IsDone = false,
            CreatedAt = DateTime.UtcNow,
        });

        // Act
        var retrievedTodoItem = await repository.GetAsync(todoItem.Entity.Id+1);

        // Assert
        Assert.Null(retrievedTodoItem);
    }


    [Fact]
    public async Task DeleteAsync_ShouldRemoveTodoItem_WhenItemWithIdExists()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext);

        var todoItem = dbContext.TodoItems.Add(new TodoItem
        {
            Title = "Test",
            IsDone = false,
            CreatedAt = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.DeleteAsync(todoItem.Entity.Id);

        // Assert
        Assert.True(result);
        var deletedTodoItem = await dbContext.TodoItems.FindAsync([todoItem.Entity.Id], TestContext.Current.CancellationToken);
        Assert.Null(deletedTodoItem);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveTodoItem_WhenItemWithIdDoesNotExist()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext);
        const int id = 999; // Non-existing ID

        // Act
        var result = await repository.DeleteAsync(id);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAllAsync_ShouldRemoveAllTodoItems()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext);

        var todoItems = new List<TodoItem>
        {
            new() { Title = "Test 1", IsDone = false, CreatedAt = DateTime.UtcNow },
            new() { Title = "Test 2", IsDone = true, CreatedAt = DateTime.UtcNow },
            new() { Title = "Test 3", IsDone = false, CreatedAt = DateTime.UtcNow }
        };

        dbContext.TodoItems.AddRange(todoItems);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var countRemoved = await repository.DeleteAllAsync();

        // Assert
        var count = await dbContext.TodoItems.CountAsync(TestContext.Current.CancellationToken);
        Assert.Equal(todoItems.Count, countRemoved);
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTodoItems()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext);

        var todoItems = new List<TodoItem>
        {
            new() { Title = "Test 1", IsDone = false, CreatedAt = DateTime.UtcNow },
            new() { Title = "Test 2", IsDone = true, CreatedAt = DateTime.UtcNow },
            new() { Title = "Test 3", IsDone = false, CreatedAt = DateTime.UtcNow }
        };

        dbContext.TodoItems.AddRange(todoItems);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var retrievedTodoItems = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(retrievedTodoItems);
        Assert.Equal(3, retrievedTodoItems.Count);
        Assert.Contains(retrievedTodoItems, item => item is { Title: "Test 1", IsDone: false });
        Assert.Contains(retrievedTodoItems, item => item is { Title: "Test 2", IsDone: true });
        Assert.Contains(retrievedTodoItems, item => item is { Title: "Test 3", IsDone: false });
    }

}