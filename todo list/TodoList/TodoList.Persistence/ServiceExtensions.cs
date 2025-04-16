using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Domain.Interfaces;
using TodoList.Persistence.Implementations;

namespace TodoList.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterPersistence(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ITodoItemRepository, SqlServerTodoItemRepository>();

        return services;
    }
}