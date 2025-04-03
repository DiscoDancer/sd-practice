namespace TodoList.Domain.Metrics.SingleActions;

public interface IItemDeletedAction
{
    void ItemDeleted(long id);
}