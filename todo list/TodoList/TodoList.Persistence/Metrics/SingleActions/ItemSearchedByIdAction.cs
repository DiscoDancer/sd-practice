using System.Diagnostics.Metrics;
using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Persistence.Metrics.SingleActions;

internal sealed class ItemSearchedByIdAction(Meter meter, string namePrefix)
    : BaseItemAction(meter, namePrefix, "searchedById"), IItemSearchedByIdAction
{
    public void ItemSearchedById(long id, bool result)
    {
        const int quantity = 1;

        Counter.Add(quantity,
            CreateProperty("id", id),
            CreateProperty("result", result)
        );
    }
}