using System.Diagnostics.Metrics;
using TodoList.Domain;
using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Persistence.Metrics.SingleActions;

internal sealed class ItemRetrievedAction(Meter meter, string namePrefix) : BaseItemAction(meter, namePrefix, "retrieved"), IItemRetrievedAction, IItemsRetrievedAction
{
    public void ItemRetrieved(TodoItem item)
    {
        const int quantity = 1;

        Counter.Add(quantity,
            CreateProperty("title", item.Title),
            CreateProperty("isDone", item.IsDone),
            CreateProperty("createdAt", item.CreatedAt),
            CreateProperty("id", item.Id)
        );
    }

    public void ItemsRetrieved(int count)
    {
        Counter.Add(count);
    }
}