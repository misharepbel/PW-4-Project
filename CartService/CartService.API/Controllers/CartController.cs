using CartService.Application.Commands;
using CartService.Application.DTOs;
using CartService.Application.Queries;
using CartService.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace CartService.API.Controllers;

[ApiController]
[Route("")]
[Authorize]
public class CartController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    private Guid CurrentUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Health check", Description = "Access: Public")]
    public IActionResult Get() => Ok("CartService is healthy.");

    [HttpGet("cached")]
    [SwaggerOperation(Summary = "Show cached products", Description = "Access: User & Admin")]
    public IActionResult GetCachedProducts([FromServices] IProductCache cache)
    {
        var cachedItems = cache;
        return Ok(cachedItems);
    }

    [HttpGet("mycart")]
    [SwaggerOperation(Summary = "Get current user's cart", Description = "Access: User & Admin")]
    public async Task<ActionResult<CartDto?>> GetMyCart()
        => Ok(await _mediator.Send(new GetCartQuery(CurrentUserId())));

    [HttpPost("item")]
    [SwaggerOperation(Summary = "Add item to cart", Description = "Access: User & Admin")]
    public async Task<IActionResult> AddItem([FromBody] CartItemDto item)
    {
        await _mediator.Send(new AddItemCommand(CurrentUserId(), item));
        return Ok();
    }

    [HttpDelete("item/{productId}")]
    [SwaggerOperation(Summary = "Remove item from cart", Description = "Access: User & Admin")]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        await _mediator.Send(new RemoveItemCommand(CurrentUserId(), productId));
        return NoContent();
    }

    [HttpDelete("mycart")]
    [SwaggerOperation(Summary = "Clear current cart", Description = "Access: User & Admin")]
    public async Task<IActionResult> Clear()
    {
        await _mediator.Send(new ClearCartCommand(CurrentUserId()));
        return NoContent();
    }

    [HttpPost("checkout")]
    [SwaggerOperation(Summary = "Checkout cart", Description = "Access: User & Admin")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutInfoDto info)
    {
        await _mediator.Send(new CheckoutCartCommand(CurrentUserId(), info));
        return Ok();
    }
}
