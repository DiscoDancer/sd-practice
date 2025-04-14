using System.Runtime.ConstrainedExecution;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using TodoList.App.Middlewares;
using TodoList.Persistence;
using TodoList.Persistence.Implementations.Models;

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

builder.Services.RegisterPersistence(builder.Configuration);

builder.Services.AddOpenTelemetry()
    .WithMetrics(providerBuilder =>
    {
        providerBuilder.AddPrometheusExporter();

        providerBuilder.AddMeter("Microsoft.AspNetCore.Hosting",
            "Microsoft.AspNetCore.Server.Kestrel", "TodoList.App");
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

// Serilog Configuration in code
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
    .MinimumLevel.Override("System", LogEventLevel.Debug)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
    )
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "aspnet-logs-{0:yyyy.MM.dd}",
        ModifyConnectionSettings = x => x
            .BasicAuthentication("elastic", "mOfkmnUdds3y-+rarQNc")
            .ServerCertificateValidationCallback((o, certificate, chain, errors) => true)
    })
    .CreateLogger();

builder.Host.UseSerilog();


//builder.Host.UseSerilog((context, services, configuration) =>
//{
//    configuration.ReadFrom.Configuration(context.Configuration)
//                 .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("https://localhost:9200"))
//                 {
//                     AutoRegisterTemplate = true,
//                     AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
//                     IndexFormat = "todoapp-logs-{0:yyyy.MM.dd}",
//                     ModifyConnectionSettings = conn =>
//                         conn.BasicAuthentication("elastic", "mOfkmnUdds3y-+rarQNc")
//                 });
//});

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