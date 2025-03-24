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
    public async Task<ActionResult<TodoItem>> Get(long id)
    {
        var item = await _repository.GetAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        return item;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TodoItem>>> GetAll()
    {
        var items = await _repository.GetAllAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult> Add(TodoItem item)
    {
        var isAdded = await _repository.AddAsync(item);
        if (!isAdded)
        {
            return BadRequest();
        }
        return Ok();
    }

    [HttpPut]
    public async Task<ActionResult> Update(TodoItem item)
    {
        var isUpdated = await _repository.UpdateAsync(item);
        if (!isUpdated)
        {
            return BadRequest();
        }
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var isDeleted = await _repository.DeleteAsync(id);
        if (!isDeleted)
        {
            return NotFound();
        }
        return Ok();
    }
}