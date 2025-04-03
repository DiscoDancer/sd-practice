namespace TodoList.Domain;

public interface ITodoItemMetrics
{
    void ItemCreated(TodoItem item);
}