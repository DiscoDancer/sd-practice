using TodoList.Client;

namespace TodoList.ConsoleApp;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:3333");

        var crudClient = new CrudClient(client);

        var todoItems = await crudClient.GetTodoItemAsync(8);

        Console.WriteLine(todoItems);
    }
}