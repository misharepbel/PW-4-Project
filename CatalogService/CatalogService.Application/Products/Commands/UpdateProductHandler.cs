using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Application.Products.Commands;
using CatalogService.Domain.Interfaces;
using CatalogService.Application.Interfaces;
using System.Linq;
using MediatR;

namespace CatalogService.Application.Products.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;
    private readonly IProductCacheProducer _producer;

    public UpdateProductHandler(ICatalogRepository repository, IMapper mapper, IProductCacheProducer producer)
    {
        _repository = repository;
        _mapper = mapper;
        _producer = producer;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null) throw new Exception("Product not found");

        product.Name = request.Name;
        product.Ean = request.Ean;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.Country = request.Country;
        product.SKU = request.SKU;
        product.Updated_at = DateTime.UtcNow;

        var result = await _repository.UpdateAsync(product);

        var all = await _repository.GetAllProductsAsync();
        var dto = all.Select(p => _mapper.Map<ProductDto>(p)).ToList();
        await _producer.PublishAsync(new ProductCacheEvent { Products = dto }, cancellationToken);

        return _mapper.Map<ProductDto>(result);
    }
}
