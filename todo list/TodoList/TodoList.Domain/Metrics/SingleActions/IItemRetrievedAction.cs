namespace TodoList.Domain.Metrics.SingleActions;

public interface IItemRetrievedAction
{
    void ItemRetrieved(TodoItem item);
}