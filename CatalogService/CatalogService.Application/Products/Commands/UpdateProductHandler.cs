using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Application.Products.Commands;
using CatalogService.Domain.Interfaces;
using MediatR;

namespace CatalogService.Application.Products.Handlers;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;

    public UpdateProductHandler(ICatalogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
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
        return _mapper.Map<ProductDto>(result);
    }
}
