using System.Diagnostics.Metrics;
using TodoList.Domain.Metrics;
using TodoList.Persistence.Implementations.Metrics.SingleActions;

namespace TodoList.Persistence.Implementations.Metrics;

internal class TodoItemMetrics : ITodoItemMetrics
{
    private readonly ItemDeletedAction _itemDeletedAction;

    public TodoItemMetrics(IMeterFactory meterFactory)
    {
        const string namePrefix = "todoList.item";
        var meter = meterFactory.Create("TodoList.App");
        _itemDeletedAction = new ItemDeletedAction(meter, namePrefix);
    }

    public void ItemDeleted(long id)
    {
        _itemDeletedAction.ItemDeleted(id);
    }

    public void ItemsDeleted(int count)
    {
        _itemDeletedAction.ItemsDeleted(count);
    }
}