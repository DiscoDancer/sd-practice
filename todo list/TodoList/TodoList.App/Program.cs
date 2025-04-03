using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using TodoList.App.Middlewares;
using TodoList.Domain;
using TodoList.Domain.Metrics;
using TodoList.Persistence;
using TodoList.Persistence.Metrics;
using TodoList.Persistence.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks().AddDbContextCheck<MasterContext>();

builder.Services.AddDbContext<MasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));


builder.Services.AddScoped<ITodoItemRepository, SqlServerTodoItemRepository>();

builder.Services.AddSingleton<ITodoItemMetrics, TodoItemMetrics>();

builder.Services.AddOpenTelemetry()
    .WithMetrics(providerBuilder =>
    {
        providerBuilder.AddPrometheusExporter();

        providerBuilder.AddMeter("Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel");
        providerBuilder.AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries =
                [
                    0, 0.005, 0.01, 0.025, 0.05,
                    0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10
                ]
            });
    });

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.EnvironmentName.Equals("Docker"))
{
    app.UseMiddleware<ContainerIdHeaderMiddleware>();
}

app.MapControllers();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapHealthChecks("/healthz").DisableHttpMetrics();
app.MapPrometheusScrapingEndpoint();

app.Run();