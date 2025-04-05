using System.Diagnostics.Metrics;
using TodoList.Domain;
using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Persistence.Implementations.Metrics.SingleActions;

internal sealed class ItemRetrievedAction(Meter meter, string namePrefix) : BaseItemAction(meter, namePrefix, "retrieved"), IItemRetrievedAction, IItemsRetrievedAction
{
    public void ItemRetrieved(TodoItem item)
    {
        const int quantity = 1;

        Counter.Add(quantity,
            CreateProperty(Properties.Title, item.Title),
            CreateProperty(Properties.IsDone, item.IsDone),
            CreateProperty(Properties.CreatedAt, item.CreatedAt),
            CreateProperty(Properties.Id, item.Id)
        );
    }

    public void ItemsRetrieved(int count)
    {
        Counter.Add(count);
    }
}