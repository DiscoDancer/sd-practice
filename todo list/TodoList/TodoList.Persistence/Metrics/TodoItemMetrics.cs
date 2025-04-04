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
    private readonly ItemRetrievedAction _itemRetrievedAction;
    public const string NamePrefix = "todoList.item";

    public TodoItemMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("TodoList.App");
        _itemCreatedAction = new ItemCreatedAction(meter, NamePrefix);
        _itemUpdatedAction = new ItemUpdatedAction(meter, NamePrefix);
        _itemRetrievedAction = new ItemRetrievedAction(meter, NamePrefix);
    }

    public void ItemCreated(TodoItem item)
    {
        _itemCreatedAction.ItemCreated(item);
    }

    public void ItemUpdated(long id, string? title, bool? isDone)
    {
        _itemUpdatedAction.ItemUpdated(id, title, isDone);
    }

    public void ItemRetrieved(TodoItem item)
    {
        _itemRetrievedAction.ItemRetrieved(item);
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

    public void ItemSearchedById(long id, bool result)
    {
        throw new NotImplementedException();
    }
}