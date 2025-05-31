using AutoMapper;
using CatalogService.Application.Categories.Queries;
using CatalogService.Application.DTOs;
using CatalogService.Domain.Interfaces;
using MediatR;

namespace CatalogService.Application.Categories.Handlers;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;

    public GetAllCategoriesHandler(ICatalogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _repository.GetAllCategoriesAsync();
        return _mapper.Map<List<CategoryDto>>(categories);
    }
}
