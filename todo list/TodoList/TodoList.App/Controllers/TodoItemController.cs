using Microsoft.AspNetCore.Mvc;
using Serilog;
using TodoList.App.Dtos;
using TodoList.Domain;
using TodoList.Domain.Events;

namespace TodoList.App.Controllers;

// https://www.josephguadagno.net/2020/07/01/no-route-matches-the-supplied-values

[Route("api/[controller]")]
[ApiController]
public class TodoItemController(ITodoItemRepository repository, ILogger<TodoItemController> logger)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> Get(long id)
    {
        var item = await repository.GetAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TodoItem>>> GetAll()
    {
        var items = await repository.GetAllAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<CreatedAtActionResult> Add(AddInput input)
    {
        var item = await repository.AddAsync(input.Title, input.IsDone);

        var ev = new TodoCreatedEvent(item);

        // Then log the event
        logger.LogInformation("{EventType} {@Event}",
            nameof(TodoCreatedEvent),
            new
            {
                ev.TodoItem.Id,
                ev.TodoItem.IsDone,
                ev.TodoItem.Title,
                ev.TodoItem.CreatedAt,
            });

        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateInput input)
    {
        var isUpdated = await repository.UpdateAsync(id, input.Title, input.IsDone);
        if (!isUpdated)
        {
            return BadRequest();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var isDeleted = await repository.DeleteAsync(id);
        if (!isDeleted)
        {
            return BadRequest();
        }
        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAll()
    {
        var isDeleted = await repository.DeleteAllAsync();
        if (!isDeleted)
        {
            return BadRequest();
        }
        return NoContent();
    }
}