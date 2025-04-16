using System.Text;
using System.Text.Json;
using TodoList.App.Dtos;
using TodoList.Domain.Interfaces;

namespace TodoList.Client;

public sealed class CrudClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IReadOnlyCollection<TodoItem>> GetTodoItemsAsync()
    {
        var response = await httpClient.GetAsync("api/TodoItem");
        var data = await DeserializeResponseAsync<IReadOnlyCollection<TodoItem>>(response);
        return data;
    }

    public async Task<TodoItem> GetTodoItemAsync(int id)
    {
        var response = await httpClient.GetAsync($"api/TodoItem/{id}");
        var data = await DeserializeResponseAsync<TodoItem>(response);
        return data;
    }


    public async Task<TodoItem> CreateTodoItemAsync(AddInput input)
    {
        using var content = await ObjectToStringContentAsync(input);
        var response = await httpClient.PostAsync("api/TodoItem", content);
        var data = await DeserializeResponseAsync<TodoItem>(response);

        return data;
    }

    public async Task UpdateTodoItemAsync(int id, UpdateInput input)
    {
        using var content = await ObjectToStringContentAsync(input);
        var response = await httpClient.PutAsync($"api/TodoItem/{id}", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTodoItemAsync(int id)
    {
        var response = await httpClient.DeleteAsync($"api/TodoItem/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAllTodoItemsAsync()
    {
        var response = await httpClient.DeleteAsync($"api/TodoItem");
        response.EnsureSuccessStatusCode();
    }

    private static async Task<StringContent> ObjectToStringContentAsync(object obj)
    {
        await using var inputStream = await SerializeObjectToStreamAsync(obj);
        using var reader = new StreamReader(inputStream);
        var json = await reader.ReadToEndAsync();

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return content;
    }

    private static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<T>(stream, JsonSerializerOptions);

        if (data is null)
        {
            throw new InvalidOperationException("Failed to deserialize the response.");
        }

        return data;
    }

    private static async Task<Stream> SerializeObjectToStreamAsync<T>(T obj)
    {
        var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, obj);
        stream.Position = 0; // Reset stream position for reading
        return stream;
    }
}