using System.Diagnostics.Metrics;
using TodoList.Domain;

namespace TodoList.App.Metrics;

public class TodoItemMetrics : ITodoItemMetrics
{
    private readonly Counter<int> _todoItemCreatedCounter;

    public TodoItemMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("TodoList.App");
        _todoItemCreatedCounter = meter.CreateCounter<int>("todoList.item.created");
    }

    public void ItemCreated(TodoItem item)
    {
        const int quantity = 1;

        _todoItemCreatedCounter.Add(quantity,
            new KeyValuePair<string, object?>("todoList.item.title", item.Title),
            new KeyValuePair<string, object?>("todoList.item.isDone", item.IsDone),
            new KeyValuePair<string, object?>("todoList.item.createdAt", item.CreatedAt),
            new KeyValuePair<string, object?>("todoList.item.id", item.Id)
            );
    }
}