using Microsoft.EntityFrameworkCore;
using TodoList.App.Middlewares;
using TodoList.Domain;
using TodoList.Persistence;
using TodoList.Persistence.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MasterContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));


builder.Services.AddScoped<ITodoItemRepository, SqlServerTodoItemRepository>();

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

app.Run();
