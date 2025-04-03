namespace TodoList.Domain.Metrics.SingleActions;

public interface IAllItemsRetrievedAction
{
    void AllItemsRetrieved(int count);
}