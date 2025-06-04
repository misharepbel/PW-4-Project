using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Application.Products.Commands;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using CatalogService.Application.Interfaces;
using System.Linq;
using MediatR;

namespace CatalogService.Application.Products.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICatalogDbContext _context;
    private readonly IProductCacheProducer _producer;
    public CreateProductHandler(ICatalogRepository repository, IMapper mapper, ICatalogDbContext context, IProductCacheProducer producer)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
        _producer = producer;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null)
            throw new ArgumentException($"Category with ID {request.CategoryId} does not exist.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Ean = request.Ean,
            Price = request.Price,
            Stock = request.Stock,
            SKU = request.SKU,
            Country = request.Country,
            Category = category
        };

        var result = await _repository.AddProductAsync(product);

        var all = await _repository.GetAllProductsAsync();
        var dto = all.Select(p => _mapper.Map<ProductDto>(p)).ToList();
        await _producer.PublishAsync(new ProductCacheEvent { Products = dto }, cancellationToken);

        return _mapper.Map<ProductDto>(result);
    }
}
