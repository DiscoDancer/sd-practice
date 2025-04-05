using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Domain.Metrics;
using TodoList.Domain;
using TodoList.Persistence.Implementations;
using TodoList.Persistence.Implementations.Metrics;

namespace TodoList.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ITodoItemRepository, SqlServerTodoItemRepository>();

        services.AddSingleton<ITodoItemMetrics, TodoItemMetrics>();

        return services;
    }
}