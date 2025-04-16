using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Domain.Metrics;

public interface ITodoItemMetrics : IItemUpdatedAction, IItemRetrievedAction, IItemDeletedAction, IItemsRetrievedAction, IItemsDeletedAction, IItemSearchedByIdAction;