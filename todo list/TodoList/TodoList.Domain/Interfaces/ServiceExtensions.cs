using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoList.Domain.Implementations;

namespace TodoList.Domain.Interfaces;

public static class ServiceExtensions
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<ITodoItemService, TodoItemService>();

        return services;
    }
}