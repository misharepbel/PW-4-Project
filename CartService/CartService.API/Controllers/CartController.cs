using CartService.Application.Commands;
using CartService.Application.DTOs;
using CartService.Application.Queries;
using CartService.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CartService.API.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class CartController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    private Guid CurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("cached")]
    public IActionResult GetCachedProducts([FromServices] IProductCache cache)
    {
        var cachedItems = cache;
        return Ok(cachedItems);
    }

    [HttpGet]
    public async Task<ActionResult<CartDto?>> Get()
        => Ok(await _mediator.Send(new GetCartQuery(CurrentUserId())));

    [HttpPost("item")]
    public async Task<IActionResult> AddItem([FromBody] CartItemDto item)
    {
        await _mediator.Send(new AddItemCommand(CurrentUserId(), item));
        return Ok();
    }

    [HttpDelete("item/{productId}")]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        await _mediator.Send(new RemoveItemCommand(CurrentUserId(), productId));
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Clear()
    {
        await _mediator.Send(new ClearCartCommand(CurrentUserId()));
        return NoContent();
    }
}
