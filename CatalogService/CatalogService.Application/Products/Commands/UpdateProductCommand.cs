using CatalogService.Application.DTOs;
using MediatR;

namespace CatalogService.Application.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Ean,
    decimal Price,
    int Stock,
    string Country,
    string SKU
) : IRequest<ProductDto>;
