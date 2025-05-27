using CatalogService.Application.Categories.Commands;
using CatalogService.Application.Categories.Queries;
using CatalogService.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[Route("[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoriesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> Get() =>
        Ok(await _mediator.Send(new GetAllCategoriesQuery()));

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Post([FromBody] CreateCategoryCommand command) =>
        Ok(await _mediator.Send(command));
}
