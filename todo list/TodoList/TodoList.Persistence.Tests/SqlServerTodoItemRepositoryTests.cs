﻿using Microsoft.EntityFrameworkCore;
using Moq;
using TodoList.Domain;
using TodoList.Domain.Metrics;
using TodoList.Persistence.Implementations;
using TodoList.Persistence.Implementations.Models;
using TodoItem = TodoList.Persistence.Implementations.Models.TodoItem;

namespace TodoList.Persistence.Tests;

public class SqlServerTodoItemRepositoryTests
{
    private readonly Mock<ITodoItemMetrics> _todoItemMetrics = new();

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
        var repository = new SqlServerTodoItemRepository(dbContext, _todoItemMetrics.Object);

        // Act
        var todoItem = await repository.AddAsync("Test", false);

        // Assert
        Assert.NotNull(todoItem);
        Assert.NotEqual(0, todoItem.Id);
        Assert.Equal("Test", todoItem.Title);
        Assert.False(todoItem.IsDone);

        var savedTodoItem = await dbContext.TodoItems.FindAsync(todoItem.Id);
        Assert.NotNull(savedTodoItem);
        Assert.Equal(todoItem.Id, savedTodoItem.Id);
        Assert.Equal(todoItem.Title, savedTodoItem.Title);
        Assert.Equal(todoItem.IsDone, savedTodoItem.IsDone);
        Assert.Equal(todoItem.CreatedAt, savedTodoItem.CreatedAt);
        _todoItemMetrics.Verify(x => x.ItemCreated(todoItem), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTodoItem()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext, _todoItemMetrics.Object);

        var todoItem = dbContext.TodoItems.Add(new TodoItem
        {
            Title = "Initial Title",
            IsDone = false,
            CreatedAt = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync();

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
        var retrievedTodoItem = await dbContext.TodoItems.FindAsync(todoItem.Entity.Id);
        Assert.NotNull(retrievedTodoItem);
        Assert.Equal(updatedTodoItem.Id, retrievedTodoItem.Id);
        Assert.Equal(updatedTodoItem.Title, retrievedTodoItem.Title);
        Assert.Equal(updatedTodoItem.IsDone, retrievedTodoItem.IsDone);
        Assert.Equal(updatedTodoItem.CreatedAt, retrievedTodoItem.CreatedAt);
        _todoItemMetrics.Verify(x => x.ItemSearchedById(todoItem.Entity.Id, true));
        _todoItemMetrics.Verify(x => x.ItemUpdated(todoItem.Entity.Id, updatedTodoItem.Title, updatedTodoItem.IsDone), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateReturnFalse_WhenItemNotFoundById()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext, _todoItemMetrics.Object);
        // Act
        var result = await repository.UpdateAsync(999, "Updated Title", true);
        // Assert
        Assert.False(result);
        _todoItemMetrics.Verify(x => x.ItemUpdated(999, "Updated Title", true), Times.Never);
        _todoItemMetrics.Verify(x => x.ItemSearchedById(999, false), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTodoItem()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext, _todoItemMetrics.Object);

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
        _todoItemMetrics.Verify(x => x.ItemRetrieved(retrievedTodoItem));
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveTodoItem_WhenItemWithIdExists()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext, _todoItemMetrics.Object);

        var todoItem = dbContext.TodoItems.Add(new TodoItem
        {
            Title = "Test",
            IsDone = false,
            CreatedAt = DateTime.UtcNow,
        });
        await dbContext.SaveChangesAsync();

        // Act
        var result = await repository.DeleteAsync(todoItem.Entity.Id);

        // Assert
        Assert.True(result);
        var deletedTodoItem = await dbContext.TodoItems.FindAsync(todoItem.Entity.Id);
        Assert.Null(deletedTodoItem);
        _todoItemMetrics.Verify(x => x.ItemDeleted(todoItem.Entity.Id), Times.Once);
        _todoItemMetrics.Verify(x => x.ItemSearchedById(todoItem.Entity.Id, true), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveTodoItem_WhenItemWithIdDoesNotExist()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext, _todoItemMetrics.Object);
        const int id = 999; // Non-existing ID

        // Act
        var result = await repository.DeleteAsync(id);

        // Assert
        Assert.False(result);
        _todoItemMetrics.Verify(x => x.ItemDeleted(id), Times.Never);
        _todoItemMetrics.Verify(x => x.ItemSearchedById(id, false), Times.Once);
    }

    [Fact]
    public async Task DeleteAllAsync_ShouldRemoveAllTodoItems()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext, _todoItemMetrics.Object);

        var todoItems = new List<TodoItem>
        {
            new() { Title = "Test 1", IsDone = false, CreatedAt = DateTime.UtcNow },
            new() { Title = "Test 2", IsDone = true, CreatedAt = DateTime.UtcNow },
            new() { Title = "Test 3", IsDone = false, CreatedAt = DateTime.UtcNow }
        };

        dbContext.TodoItems.AddRange(todoItems);
        await dbContext.SaveChangesAsync();

        // Act
        await repository.DeleteAllAsync();

        // Assert
        var count = await dbContext.TodoItems.CountAsync();
        Assert.Equal(0, count);
        _todoItemMetrics.Verify(x => x.ItemsDeleted(todoItems.Count));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTodoItems()
    {
        // Arrange
        var options = GetInMemoryOptions();
        await using var dbContext = new MasterContext(options);
        var repository = new SqlServerTodoItemRepository(dbContext, _todoItemMetrics.Object);

        var todoItems = new List<TodoItem>
        {
            new() { Title = "Test 1", IsDone = false, CreatedAt = DateTime.UtcNow },
            new() { Title = "Test 2", IsDone = true, CreatedAt = DateTime.UtcNow },
            new() { Title = "Test 3", IsDone = false, CreatedAt = DateTime.UtcNow }
        };

        dbContext.TodoItems.AddRange(todoItems);
        await dbContext.SaveChangesAsync();

        // Act
        var retrievedTodoItems = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(retrievedTodoItems);
        Assert.Equal(3, retrievedTodoItems.Count);
        Assert.Contains(retrievedTodoItems, item => item is { Title: "Test 1", IsDone: false });
        Assert.Contains(retrievedTodoItems, item => item is { Title: "Test 2", IsDone: true });
        Assert.Contains(retrievedTodoItems, item => item is { Title: "Test 3", IsDone: false });
        _todoItemMetrics.Verify(x => x.ItemsRetrieved(todoItems.Count));
    }

}