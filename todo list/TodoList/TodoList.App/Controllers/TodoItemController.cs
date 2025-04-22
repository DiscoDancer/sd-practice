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
    //[HttpGet("{id}")]
    //public async Task<ActionResult<TodoItem>> Get(long id, CancellationToken cancellationToken)
    //{
    //    var result = await service.AccessAsync(id, cancellationToken);
    //    if (result.IsFailure)
    //    {
    //        logger.LogError(result.Error);
    //        return BadRequest(result.Error);
    //    }

    //    var resultEvent = result.Value;
    //    if (resultEvent.Result == AccessResult.NotFound || resultEvent.Item == null)
    //    {
    //        logger.LogInformation("{EventType} {@Event}",
    //            nameof(TodoAccessedEvent),
    //            new
    //            {
    //                resultEvent.Id,
    //                resultEvent.Result
    //            });
    //        return NotFound();
    //    }

    //    logger.LogInformation("{EventType} {@Event}",
    //        nameof(TodoAccessedEvent),
    //        new
    //        {
    //            resultEvent.Id,
    //            resultEvent.Item.CreatedAt,
    //            resultEvent.Item.IsDone,
    //            resultEvent.Item.Title,
    //            resultEvent.Result
    //        });

    //    return Ok(resultEvent.Item);
    //}

    //[HttpGet]
    //public async Task<ActionResult<IReadOnlyCollection<TodoItem>>> GetAll(CancellationToken cancellationToken)
    //{
    //    var result = await service.AccessAllAsync(cancellationToken);
    //    if (result.IsFailure)
    //    {
    //        result.Error.LogError(logger);
    //        return BadRequest(result.Error);
    //    }

    //    var resultEvent = result.Value;

    //    logger.LogInformation("{EventType} {@Event}",
    //        nameof(TodoAccessedAllEvent),
    //        new
    //        {
    //            resultEvent.Items.Count,
    //        });

    //    if (resultEvent.Items.Count == 0)
    //    {
    //        return NotFound();
    //    }

    //    return Ok(resultEvent.Items);
    //}

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

        throw new NotImplementedException();

        // return CreatedAtAction(nameof(Get), new { id = todoItem.Id }, todoItem);
    }

    //[HttpPut("{id}")]
    //public async Task<ActionResult> Update(long id, [FromBody] UpdateInput input, CancellationToken cancellationToken)
    //{
    //    var result = await service.UpdateAsync(id, input.Title, input.IsDone, cancellationToken);
    //    if (result.IsFailure)
    //    {
    //        logger.LogError(result.Error);
    //        return BadRequest(result.Error);
    //    }

    //    logger.LogInformation("{EventType} {@Event}",
    //        nameof(TodoUpdatedEvent),
    //        new
    //        {
    //            result.Value.UpdateStatus,
    //            result.Value.Id,
    //            result.Value.IsDone,
    //            result.Value.Title,
    //        });
        
    //    if (result.Value.UpdateStatus == UpdateResult.NotUpdated)
    //    {
    //        return BadRequest();
    //    }

    //    return NoContent();
    //}

    //[HttpDelete("{id}")]
    //public async Task<ActionResult> Delete(long id, CancellationToken cancellationToken)
    //{
    //    var result = await service.DeleteAsync(id, cancellationToken);
    //    if (result.IsFailure)
    //    {
    //        logger.LogError(result.Error);
    //        return BadRequest(result.Error);
    //    }

    //    logger.LogInformation("{EventType} {@Event}",
    //        nameof(TodoDeletedEvent),
    //        new
    //        {
    //            result.Value.DeleteResult,
    //            result.Value.Id,
    //        });

    //    return NoContent();
    //}

    //[HttpDelete]
    //public async Task<ActionResult> DeleteAll(CancellationToken cancellationToken)
    //{
    //    var result = await service.DeleteAllAsync(cancellationToken);
    //    if (result.IsFailure)
    //    {
    //        logger.LogError(result.Error);
    //        return BadRequest(result.Error);
    //    }

    //    logger.LogInformation("{EventType} {@Event}",
    //        nameof(TodoDeletedAllEvent),
    //        new
    //        {
    //            result.Value.Count,
    //        });

    //    var deletedCount = result.Value.Count;

    //    if (deletedCount == 0)
    //    {
    //        return BadRequest();
    //    }
    //    return NoContent();
    //}
}