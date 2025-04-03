using System.Diagnostics.Metrics;
using TodoList.Domain;
using TodoList.Domain.Metrics;
using TodoList.Domain.Metrics.SingleActions;
using TodoList.Persistence.Metrics.SingleActions;

namespace TodoList.Persistence.Metrics;

public class TodoItemMetrics : ITodoItemMetrics
{
    private readonly IItemCreatedAction _itemCreatedAction;
    private readonly IItemUpdatedAction _itemUpdatedAction;
    public const string NamePrefix = "todoList.item";

    public TodoItemMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("TodoList.App");
        _itemCreatedAction = new ItemCreatedAction(meter, NamePrefix);
        _itemUpdatedAction = new ItemUpdatedAction(meter, NamePrefix);
    }

    public void ItemCreated(TodoItem item)
    {
        _itemCreatedAction.ItemCreated(item);
    }

    public void ItemUpdated(TodoItem item)
    {
        _itemUpdatedAction.ItemUpdated(item);
    }

    public void ItemRetrieved(TodoItem item)
    {
        throw new NotImplementedException();
    }

    public void ItemDeleted(long id)
    {
        throw new NotImplementedException();
    }

    public void AllItemsRetrieved(int count)
    {
        throw new NotImplementedException();
    }

    public void AllItemsDeleted(int count)
    {
        throw new NotImplementedException();
    }
}