using Microsoft.AspNetCore.Mvc;
using TodoList.Domain;

namespace TodoList.App.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TodoItemController : ControllerBase
{
    private readonly ITodoItemRepository _repository;
    public TodoItemController(ITodoItemRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetAsync(long id)
    {
        var item = await _repository.GetAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TodoItem>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult> AddAsync(TodoItem item)
    {
        var isAdded = await _repository.AddAsync(item);
        if (!isAdded)
        {
            return BadRequest();
        }
        return CreatedAtAction(nameof(GetAsync), new { id = item.Id }, item);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateAsync(TodoItem item)
    {
        var isUpdated = await _repository.UpdateAsync(item);
        if (!isUpdated)
        {
            return BadRequest();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(long id)
    {
        var isDeleted = await _repository.DeleteAsync(id);
        if (!isDeleted)
        {
            return BadRequest();
        }
        return NoContent();
    }
}