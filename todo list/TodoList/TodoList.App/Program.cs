using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using TodoList.App.Middlewares;
using TodoList.Domain.Interfaces;
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
builder.Services.RegisterDomainServices(builder.Configuration);


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


builder.Host.UseSerilog((context, services, configuration) =>
{
    var elasticUri = context.Configuration["Elastic:Uri"];
    var elasticUsername = context.Configuration["Elastic:Username"];
    var elasticPassword = context.Configuration["Elastic:Password"];

    ArgumentException.ThrowIfNullOrWhiteSpace(elasticUri);
    ArgumentException.ThrowIfNullOrWhiteSpace(elasticUsername);
    ArgumentException.ThrowIfNullOrWhiteSpace(elasticPassword);

    var containerName = Environment.GetEnvironmentVariable("CONTAINER_ALIAS") ?? Environment.GetEnvironmentVariable("HOSTNAME") ?? Environment.MachineName;

    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithProperty("ContainerAlias", containerName)
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
        {
            // disable cert check, not possible to do in appsettings.json
            ModifyConnectionSettings = x => x
                .BasicAuthentication(elasticUsername, elasticPassword)
                .ServerCertificateValidationCallback((o, certificate, chain, errors) => true),
            AutoRegisterTemplate = true,
            IndexFormat = "aspnet-logs-{0:yyyy.MM.dd}"
        })
        .WriteTo.Console()
        .WriteTo.File(
            "Logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate:
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        );
});

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.EnvironmentName.Equals("Docker")) app.UseMiddleware<ContainerIdHeaderMiddleware>();

app.MapControllers();

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.MapHealthChecks("/healthz").DisableHttpMetrics();
app.MapPrometheusScrapingEndpoint();

app.Run();