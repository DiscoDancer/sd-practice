using TodoList.Domain;

namespace TodoList.App.Metrics;

public interface ITodoItemMetrics
{
    void ItemCreated(TodoItem item);
}