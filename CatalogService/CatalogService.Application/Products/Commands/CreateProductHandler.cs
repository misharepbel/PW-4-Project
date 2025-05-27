using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Application.Products.Commands;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using MediatR;

namespace CatalogService.Application.Products.Handlers;

public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;
    private readonly ICatalogDbContext _context;

    public CreateProductHandler(ICatalogRepository repository, IMapper mapper, ICatalogDbContext context)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(request.CategoryId);
        if (category == null)
            throw new ArgumentException($"Category with ID {request.CategoryId} does not exist.");

        var product = new Product
        {
            Name = request.Name,
            Ean = request.Ean,
            Price = request.Price,
            Stock = request.Stock,
            SKU = request.SKU,
            Country = request.Country,
            Category = category
        };

        var result = await _repository.AddProductAsync(product);
        return _mapper.Map<ProductDto>(result);
    }
}
