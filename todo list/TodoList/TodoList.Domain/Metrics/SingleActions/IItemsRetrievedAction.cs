namespace TodoList.Domain.Metrics.SingleActions;

public interface IItemsRetrievedAction
{
    void ItemsRetrieved(int count);
}