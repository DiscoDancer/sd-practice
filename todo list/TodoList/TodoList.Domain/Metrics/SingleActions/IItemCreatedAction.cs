namespace TodoList.Domain.Metrics.SingleActions;

public interface IItemCreatedAction
{
    void ItemCreated(TodoItem item);
}