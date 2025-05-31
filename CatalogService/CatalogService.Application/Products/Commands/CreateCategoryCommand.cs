using CatalogService.Application.DTOs;
using MediatR;

namespace CatalogService.Application.Categories.Commands;

public record CreateCategoryCommand(string Name) : IRequest<CategoryDto>;
