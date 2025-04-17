using FluentAssertions;
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
        todoItem.Should().NotBeNull();
        todoItem.Id.Should().NotBe(0);
        todoItem.Title.Should().Be("Test");
        todoItem.IsDone.Should().BeFalse();

        var savedTodoItem = await dbContext.TodoItems.FindAsync([todoItem.Id], TestContext.Current.CancellationToken);
        savedTodoItem.Should().NotBeNull();
        savedTodoItem.Id.Should().Be(todoItem.Id);
        savedTodoItem.Title.Should().Be(todoItem.Title);
        savedTodoItem.IsDone.Should().Be(todoItem.IsDone);
        savedTodoItem.CreatedAt.Should().Be(todoItem.CreatedAt);
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

        retrievedTodoItem.Should().NotBeNull();
        retrievedTodoItem.Id.Should().Be(updatedTodoItem.Id);
        retrievedTodoItem.Title.Should().Be(updatedTodoItem.Title);
        retrievedTodoItem.IsDone.Should().Be(updatedTodoItem.IsDone);
        retrievedTodoItem.CreatedAt.Should().Be(updatedTodoItem.CreatedAt);
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
        result.Should().BeFalse();
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
        retrievedTodoItem.Should().NotBeNull();
        retrievedTodoItem.Id.Should().Be(todoItem.Entity.Id);
        retrievedTodoItem.Title.Should().Be(todoItem.Entity.Title);
        retrievedTodoItem.IsDone.Should().Be(todoItem.Entity.IsDone);
        retrievedTodoItem.CreatedAt.Should().Be(todoItem.Entity.CreatedAt);
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
        var retrievedTodoItem = await repository.GetAsync(todoItem.Entity.Id + 1);

        // Assert
        retrievedTodoItem.Should().BeNull();
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
        result.Should().BeTrue();
        var deletedTodoItem = await dbContext.TodoItems.FindAsync([todoItem.Entity.Id], TestContext.Current.CancellationToken);
        deletedTodoItem.Should().BeNull();
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
        result.Should().BeFalse();
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
        countRemoved.Should().Be(todoItems.Count);
        var count = await dbContext.TodoItems.CountAsync(TestContext.Current.CancellationToken);
        count.Should().Be(0);
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
        retrievedTodoItems.Should().NotBeNull();
        retrievedTodoItems.Should().HaveCount(3);
        retrievedTodoItems.Should().ContainSingle(item => item.Title == "Test 1" && !item.IsDone);
        retrievedTodoItems.Should().ContainSingle(item => item.Title == "Test 2" && item.IsDone);
        retrievedTodoItems.Should().ContainSingle(item => item.Title == "Test 3" && !item.IsDone);
    }
}