using CatalogService.Application.DTOs;
using CatalogService.Application.Products.Commands;
using CatalogService.Application.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ProductsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<ActionResult<List<ProductDto>>> Get() =>
            Ok(await _mediator.Send(new GetAllProductsQuery()));

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(Guid id) =>
            Ok(await _mediator.Send(new GetProductByIdQuery(id)));

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> Post([FromBody] CreateProductCommand command) =>
            Ok(await _mediator.Send(command));

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> Put(Guid id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id) return BadRequest("Id mismatch");
            return Ok(await _mediator.Send(command));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteProductCommand(id));
            return result ? Ok() : NotFound();
        }
    }
}