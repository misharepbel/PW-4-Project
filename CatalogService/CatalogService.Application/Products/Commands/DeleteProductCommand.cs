using MediatR;

namespace CatalogService.Application.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest<bool>;
