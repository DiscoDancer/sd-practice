using System.Text.Json;
using TodoList.Domain;

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
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<IReadOnlyCollection<TodoItem>>(stream, JsonSerializerOptions);

        if (data is null)
        {
            throw new InvalidOperationException("Failed to deserialize the response.");
        }

        return data;
    }

    public async Task<TodoItem> GetTodoItemAsync(int id)
    {
        var response = await httpClient.GetAsync($"api/TodoItem/{id}");
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var data = await JsonSerializer.DeserializeAsync<TodoItem>(stream, JsonSerializerOptions);

        if (data is null)
        {
            throw new InvalidOperationException("Failed to deserialize the response.");
        }

        return data;
    }


    //public async Task<TodoItem> CreateTodoItemAsync(TodoItem todoItem)
    //{
    //    var content = new StringContent(JsonSerializer.Serialize(todoItem), Encoding.UTF8, "application/json");
    //    var response = await _httpClient.PostAsync("api/todoitems", content);
    //    response.EnsureSuccessStatusCode();
    //    var responseContent = await response.Content.ReadAsStringAsync();
    //    return JsonSerializer.Deserialize<TodoItem>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    //}
    //public async Task UpdateTodoItemAsync(int id, TodoItem todoItem)
    //{
    //    var content = new StringContent(JsonSerializer.Serialize(todoItem), Encoding.UTF8, "application/json");
    //    var response = await _httpClient.PutAsync($"api/todoitems/{id}", content);
    //    response.EnsureSuccessStatusCode();
    //}
    //public async Task DeleteTodoItemAsync(int id)
    //{
    //    var response = await _httpClient.DeleteAsync($"api/todoitems/{id}");
    //    response.EnsureSuccessStatusCode();
    //}
}