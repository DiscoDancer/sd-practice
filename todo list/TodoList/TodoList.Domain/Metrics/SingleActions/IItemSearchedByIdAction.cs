namespace TodoList.Domain.Metrics.SingleActions;

public interface IItemSearchedByIdAction
{
    void ItemSearchedById(long id, bool result);
}