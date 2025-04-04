﻿using System.Diagnostics.Metrics;
using TodoList.Domain;
using TodoList.Domain.Metrics;
using TodoList.Persistence.Implementations.Metrics.SingleActions;

namespace TodoList.Persistence.Implementations.Metrics;

internal class TodoItemMetrics : ITodoItemMetrics
{
    private readonly ItemCreatedAction _itemCreatedAction;
    private readonly ItemUpdatedAction _itemUpdatedAction;
    private readonly ItemRetrievedAction _itemRetrievedAction;
    private readonly ItemDeletedAction _itemDeletedAction;
    private readonly ItemSearchedByIdAction _itemSearchedByIdAction;

    public TodoItemMetrics(IMeterFactory meterFactory)
    {
        const string namePrefix = "todoList.item";
        var meter = meterFactory.Create("TodoList.App");
        _itemCreatedAction = new ItemCreatedAction(meter, namePrefix);
        _itemUpdatedAction = new ItemUpdatedAction(meter, namePrefix);
        _itemRetrievedAction = new ItemRetrievedAction(meter, namePrefix);
        _itemDeletedAction = new ItemDeletedAction(meter, namePrefix);
        _itemSearchedByIdAction = new ItemSearchedByIdAction(meter, namePrefix);
    }

    public void ItemCreated(TodoItem item)
    {
        _itemCreatedAction.ItemCreated(item);
    }

    public void ItemUpdated(long id, string? title, bool? isDone)
    {
        _itemUpdatedAction.ItemUpdated(id, title, isDone);
    }

    public void ItemRetrieved(TodoItem item)
    {
        _itemRetrievedAction.ItemRetrieved(item);
    }

    public void ItemDeleted(long id)
    {
        _itemDeletedAction.ItemDeleted(id);
    }

    public void ItemsRetrieved(int count)
    {
        _itemRetrievedAction.ItemsRetrieved(count);
    }

    public void ItemsDeleted(int count)
    {
        _itemDeletedAction.ItemsDeleted(count);
    }

    public void ItemSearchedById(long id, bool result)
    {
        _itemSearchedByIdAction.ItemSearchedById(id, result);
    }
}