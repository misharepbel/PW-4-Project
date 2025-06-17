using CatalogService.Application.Categories.Commands;
using CatalogService.Application.Categories.Queries;
using CatalogService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

[Route("[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [SwaggerOperation(Summary = "List all categories", Description = "Access: Public")]
    public async Task<ActionResult<List<CategoryDto>>> Get() =>
        Ok(await _mediator.Send(new GetAllCategoriesQuery()));

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Create a new category", Description = "Access: Admin only")]
    public async Task<ActionResult<CategoryDto>> Post([FromBody] CreateCategoryCommand command) =>
        Ok(await _mediator.Send(command));
}
