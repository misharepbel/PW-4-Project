using CatalogService.Application.DTOs;
using MediatR;

namespace CatalogService.Application.Products.Queries;

public record GetAllProductsQuery : IRequest<List<ProductDto>>;
