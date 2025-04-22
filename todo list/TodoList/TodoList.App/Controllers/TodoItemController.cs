using Microsoft.AspNetCore.Mvc;
using TodoList.App.Dtos;
using TodoList.Domain.Interfaces;
using TodoList.Domain.Interfaces.Events;

namespace TodoList.App.Controllers;

// https://www.josephguadagno.net/2020/07/01/no-route-matches-the-supplied-values

[Route("api/[controller]")]
[ApiController]
public class TodoItemController(ILogger<TodoItemController> logger, ITodoItemService service)
    : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> Get(long id, CancellationToken cancellationToken)
    {
        var result = await service.AccessAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            result.Error.LogError(logger);
            return BadRequest(result.Error);
        }

        var resultEvent = result.Value;
        resultEvent.LogInformation(logger);

        if (resultEvent.Result == AccessResult.NotFound)
        {
            return NotFound();
        }

        return Ok(resultEvent.Item);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TodoItem>>> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.AccessAllAsync(cancellationToken);
        if (result.IsFailure)
        {
            result.Error.LogError(logger);
            return BadRequest(result.Error);
        }

        var resultEvent = result.Value;
        resultEvent.LogInformation(logger);

        if (resultEvent.Items.Count == 0)
        {
            return NotFound();
        }

        return Ok(resultEvent.Items);
    }

    [HttpPost]
    public async Task<IActionResult> Add(AddInput input, CancellationToken cancellationToken)
    {
        var result = await service.AddAsync(input.Title, input.IsDone, cancellationToken);
        if (result.IsFailure)
        {
            result.Error.LogError(logger);
            return BadRequest(result.Error);
        }

        var resultEvent = result.Value;
        var todoItem = resultEvent.TodoItem;
        resultEvent.LogInformation(logger);

        return CreatedAtAction(nameof(Get), new { id = todoItem.Id }, todoItem);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateInput input, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, input.Title, input.IsDone, cancellationToken);
        if (result.IsFailure)
        {
            result.Error.LogError(logger);
            return BadRequest(result.Error);
        }

        result.Value.LogInformation(logger);

        if (result.Value.UpdateStatus == UpdateResult.NotUpdated)
        {
            return BadRequest();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        if (result.IsFailure)
        {
            result.Error.LogError(logger);
            return BadRequest(result.Error);
        }

        result.Value.LogInformation(logger);

        return NoContent();
    }

    [HttpDelete]
    public async Task<ActionResult> DeleteAll(CancellationToken cancellationToken)
    {
        var result = await service.DeleteAllAsync(cancellationToken);
        if (result.IsFailure)
        {
            result.Error.LogError(logger);
            return BadRequest(result.Error);
        }

        result.Value.LogInformation(logger);

        var deletedCount = result.Value.Count;

        if (deletedCount == 0)
        {
            return BadRequest();
        }
        return NoContent();
    }
}