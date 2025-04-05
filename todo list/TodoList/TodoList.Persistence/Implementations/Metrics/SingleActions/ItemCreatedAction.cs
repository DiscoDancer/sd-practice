using System.Diagnostics.Metrics;
using TodoList.Domain;
using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Persistence.Implementations.Metrics.SingleActions;

internal sealed class ItemCreatedAction(Meter meter, string namePrefix) : BaseItemAction(meter, namePrefix, "created"), IItemCreatedAction
{
    public void ItemCreated(TodoItem item)
    {
        const int quantity = 1;

        Counter.Add(quantity,
            CreateProperty(Properties.Title, item.Title),
            CreateProperty(Properties.IsDone, item.IsDone),
            CreateProperty(Properties.CreatedAt, item.CreatedAt),
            CreateProperty(Properties.Id, item.Id)
        );
    }
}