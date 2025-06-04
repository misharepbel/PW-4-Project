using CatalogService.Application.DTOs;
using MediatR;

namespace CatalogService.Application.Categories.Queries;

public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;
