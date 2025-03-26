using Microsoft.AspNetCore.Mvc;
using TodoList.App.Dtos;
using TodoList.Domain;

namespace TodoList.App.Controllers;

// https://www.josephguadagno.net/2020/07/01/no-route-matches-the-supplied-values

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
        return Ok(item);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TodoItem>>> GetAll()
    {
        var items = await _repository.GetAllAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<CreatedAtActionResult> Add(AddInput input)
    {
        var item = await _repository.AddAsync(input.Title, input.IsDone);

        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateInput input)
    {
        var isUpdated = await _repository.UpdateAsync(id, input.Title, input.IsDone);
        if (!isUpdated)
        {
            return BadRequest();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var isDeleted = await _repository.DeleteAsync(id);
        if (!isDeleted)
        {
            return BadRequest();
        }
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAll()
    {
        var isDeleted = await _repository.DeleteAllAsync();
        if (!isDeleted)
        {
            return BadRequest();
        }
        return NoContent();
    }
}