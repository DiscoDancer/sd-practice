using System.Diagnostics.Metrics;
using TodoList.Domain;
using TodoList.Domain.Metrics;
using TodoList.Persistence.Metrics.SingleActions;

namespace TodoList.Persistence.Metrics;

public class TodoItemMetrics : ITodoItemMetrics
{
    private readonly ItemCreatedAction _itemCreatedAction;
    private readonly ItemUpdatedAction _itemUpdatedAction;
    private readonly ItemRetrievedAction _itemRetrievedAction;
    private readonly ItemDeletedAction _itemDeletedAction;
    private readonly ItemSearchedByIdAction _itemSearchedByIdAction;
    public const string NamePrefix = "todoList.item";

    public TodoItemMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("TodoList.App");
        _itemCreatedAction = new ItemCreatedAction(meter, NamePrefix);
        _itemUpdatedAction = new ItemUpdatedAction(meter, NamePrefix);
        _itemRetrievedAction = new ItemRetrievedAction(meter, NamePrefix);
        _itemDeletedAction = new ItemDeletedAction(meter, NamePrefix);
        _itemSearchedByIdAction = new ItemSearchedByIdAction(meter, NamePrefix);
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
        _itemDeletedAction.ItemDeleted(id);
    }

    public void ItemsRetrieved(int count)
    {
        _itemRetrievedAction.ItemsRetrieved(count);
    }

    public void ItemsDeleted(int count)
    {
        _itemDeletedAction.ItemsDeleted(count);
    }

    public void ItemSearchedById(long id, bool result)
    {
        _itemSearchedByIdAction.ItemSearchedById(id, result);
    }
}