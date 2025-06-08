using CatalogService.Application.Products.Commands;
using CatalogService.Domain.Interfaces;
using CatalogService.Application.Interfaces;
using CatalogService.Application.DTOs;
using System.Linq;
using MediatR;
using AutoMapper;

namespace CatalogService.Application.Products.Handlers;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly ICatalogRepository _repository;
    private readonly IProductCacheProducer _producer;
    private readonly IMapper _mapper;

    public DeleteProductHandler(ICatalogRepository repository, IProductCacheProducer producer, IMapper mapper)
    {
        _repository = repository;
        _producer = producer;
        _mapper = mapper;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null) return false;

        product.Deleted = true;
        await _repository.UpdateAsync(product);

        var all = await _repository.GetAllProductsAsync();
        var dto = all.Select(p => _mapper.Map<ProductDto>(p)).ToList();
        await _producer.PublishAsync(new ProductCacheEvent { Products = dto }, cancellationToken);

        return true;
    }
}
