namespace TodoList.Domain.Metrics.SingleActions;

public interface IItemUpdatedAction
{
    void ItemUpdated(long id, string? title, bool? isDone);
}