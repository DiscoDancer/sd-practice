namespace TodoList.Domain.Metrics.SingleActions;

public interface IAllItemsDeletedAction
{
    void AllItemsDeleted(int count);
}