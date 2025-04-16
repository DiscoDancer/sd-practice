using Microsoft.AspNetCore.Mvc;
using TodoList.App.Dtos;
using TodoList.Domain;
using TodoList.Domain.Events;
using TodoList.Domain.Services;

namespace TodoList.App.Controllers;

// https://www.josephguadagno.net/2020/07/01/no-route-matches-the-supplied-values

[Route("api/[controller]")]
[ApiController]
public class TodoItemController(ITodoItemRepository repository, ILogger<TodoItemController> logger, ITodoItemService service)
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
    public async Task<IActionResult> Add(AddInput input)
    {
        var result = await service.AddAsync(input.Title, input.IsDone);
        if (result.IsFailure || result.Value == null)
        {
            return BadRequest(result.Error);
        }

        var resultEvent = result.Value;
        var todoItem = resultEvent.TodoItem;

        logger.LogInformation("{EventType} {@Event}",
            nameof(TodoCreatedEvent),
            new
            {
                todoItem.Id,
                todoItem.IsDone,
                todoItem.Title,
                todoItem.CreatedAt,
            });

        return CreatedAtAction(nameof(Get), new { id = todoItem.Id }, todoItem);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateInput input)
    {
        var result = await service.UpdateAsync(id, input.Title, input.IsDone);
        if (result.IsFailure || result.Value == null)
        {
            return BadRequest(result.Error);
        }

        logger.LogInformation("{EventType} {@Event}",
            nameof(TodoUpdatedEvent),
            new
            {
                result.Value.UpdateStatus,
                result.Value.Id,
                result.Value.IsDone,
                result.Value.Title,
            });
        
        if (result.Value.UpdateStatus == UpdateStatus.NotUpdated)
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