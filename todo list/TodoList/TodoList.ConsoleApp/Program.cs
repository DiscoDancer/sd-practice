using TodoList.App.Dtos;
using TodoList.Client;

namespace TodoList.ConsoleApp;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:3333");

        var crudClient = new CrudClient(client);


        await crudClient.DeleteAllTodoItemsAsync();
        var result = await crudClient.GetTodoItemsAsync();
        //var result = await crudClient.CreateTodoItemAsync(new AddInput
        //{
        //    IsDone = true,
        //    Title = "Test"
        //});


        foreach (var item in result)
        {
            Console.WriteLine(item);
        }
    }
}