using MediatR;

namespace CatalogService.Application.Products.Commands;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;
