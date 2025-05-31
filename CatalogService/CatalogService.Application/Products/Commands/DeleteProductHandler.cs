using CatalogService.Application.Products.Commands;
using CatalogService.Domain.Interfaces;
using MediatR;

namespace CatalogService.Application.Products.Handlers;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly ICatalogRepository _repository;

    public DeleteProductHandler(ICatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null) return false;

        product.Deleted = true;
        await _repository.UpdateAsync(product);
        return true;
    }
}
