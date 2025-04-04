using System.Diagnostics.Metrics;
using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Persistence.Metrics.SingleActions;

internal sealed class ItemDeletedAction(Meter meter, string namePrefix) : BaseItemAction(meter, namePrefix, "deleted"), IItemDeletedAction, IItemsDeletedAction
{
    public void ItemDeleted(long id)
    {
        const int quantity = 1;

        Counter.Add(quantity,
            CreateProperty("id", id)
        );
    }

    public void ItemsDeleted(int count)
    {

        Counter.Add(count);
    }
}