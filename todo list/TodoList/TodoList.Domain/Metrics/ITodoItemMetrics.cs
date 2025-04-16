using TodoList.Domain.Metrics.SingleActions;

namespace TodoList.Domain.Metrics;

public interface ITodoItemMetrics : IItemDeletedAction, IItemsDeletedAction;