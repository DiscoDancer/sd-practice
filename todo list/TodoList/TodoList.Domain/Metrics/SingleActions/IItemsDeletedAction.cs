namespace TodoList.Domain.Metrics.SingleActions;

public interface IItemsDeletedAction
{
    void ItemsDeleted(int count);
}