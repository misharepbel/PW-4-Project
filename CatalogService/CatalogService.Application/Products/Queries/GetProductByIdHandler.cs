using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Application.Products.Queries;
using CatalogService.Domain.Interfaces;
using MediatR;

namespace CatalogService.Application.Products.Handlers;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(ICatalogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<ProductDto>(product!);
    }
}
