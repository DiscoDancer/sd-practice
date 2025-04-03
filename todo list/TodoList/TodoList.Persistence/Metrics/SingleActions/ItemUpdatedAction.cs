using System.Diagnostics.Metrics;
using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Persistence.Metrics.SingleActions;

internal sealed class ItemUpdatedAction(Meter meter, string namePrefix) : BaseItemAction(meter, namePrefix, "updated"), IItemUpdatedAction
{
    public void ItemUpdated(long id, string? title, bool? isDone)
    {
        const int quantity = 1;

        Counter.Add(quantity,
            CreateProperty("id", id),
            CreateProperty("title", title),
            CreateProperty("isDone", isDone)
        );
    }
}