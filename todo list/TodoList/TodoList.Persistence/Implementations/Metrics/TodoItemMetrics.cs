using System.Diagnostics.Metrics;
using TodoList.Domain;
using TodoList.Domain.Metrics;
using TodoList.Persistence.Implementations.Metrics.SingleActions;

namespace TodoList.Persistence.Implementations.Metrics;

internal class TodoItemMetrics : ITodoItemMetrics
{
    private readonly ItemRetrievedAction _itemRetrievedAction;
    private readonly ItemDeletedAction _itemDeletedAction;
    private readonly ItemSearchedByIdAction _itemSearchedByIdAction;

    public TodoItemMetrics(IMeterFactory meterFactory)
    {
        const string namePrefix = "todoList.item";
        var meter = meterFactory.Create("TodoList.App");
        _itemRetrievedAction = new ItemRetrievedAction(meter, namePrefix);
        _itemDeletedAction = new ItemDeletedAction(meter, namePrefix);
        _itemSearchedByIdAction = new ItemSearchedByIdAction(meter, namePrefix);
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