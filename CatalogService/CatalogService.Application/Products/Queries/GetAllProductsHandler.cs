using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Application.Products.Queries;
using CatalogService.Domain.Interfaces;
using MediatR;

namespace CatalogService.Application.Products.Handlers;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;

    public GetAllProductsHandler(ICatalogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllProductsAsync();
        return _mapper.Map<List<ProductDto>>(products);
    }
}
