using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Domain.Metrics;

public interface ITodoItemMetrics : IItemCreatedAction, IItemUpdatedAction, IItemRetrievedAction, IItemDeletedAction, IAllItemsRetrievedAction, IAllItemsDeletedAction;