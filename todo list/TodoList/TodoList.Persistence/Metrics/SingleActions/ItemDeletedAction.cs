using System.Diagnostics.Metrics;
using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Persistence.Metrics.SingleActions;

internal sealed class ItemDeletedAction(Meter meter, string namePrefix) : BaseItemAction(meter, namePrefix, "deleted"), IItemDeletedAction
{
    public void ItemDeleted(long id)
    {
        const int quantity = 1;

        Counter.Add(quantity,
            CreateProperty("id", id)
        );
    }
}