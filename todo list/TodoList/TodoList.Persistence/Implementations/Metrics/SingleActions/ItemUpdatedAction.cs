using System.Diagnostics.Metrics;
using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Persistence.Implementations.Metrics.SingleActions;

internal sealed class ItemUpdatedAction(Meter meter, string namePrefix) : BaseItemAction(meter, namePrefix, "updated"), IItemUpdatedAction
{
    public void ItemUpdated(long id, string? title, bool? isDone)
    {
        const int quantity = 1;

        Counter.Add(quantity,
            CreateProperty(Properties.Id, id),
            CreateProperty(Properties.Title, title),
            CreateProperty(Properties.IsDone, isDone)
        );
    }
}