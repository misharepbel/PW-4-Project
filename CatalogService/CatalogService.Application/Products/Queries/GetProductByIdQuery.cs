using CatalogService.Application.DTOs;
using MediatR;

namespace CatalogService.Application.Products.Queries;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto>;
