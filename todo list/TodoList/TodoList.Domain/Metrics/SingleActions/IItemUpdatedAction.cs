namespace TodoList.Domain.Metrics.SingleActions;

public interface IItemUpdatedAction
{
    void ItemUpdated(TodoItem item);
}