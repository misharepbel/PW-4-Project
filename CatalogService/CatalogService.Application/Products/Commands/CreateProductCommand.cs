using CatalogService.Application.DTOs;
using MediatR;

namespace CatalogService.Application.Products.Commands;

public record CreateProductCommand(
    string Name,
    string Ean,
    decimal Price,
    int Stock,
    string Country,
    string SKU,
    int CategoryId
) : IRequest<ProductDto>;
