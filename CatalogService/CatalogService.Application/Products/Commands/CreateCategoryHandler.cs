using AutoMapper;
using CatalogService.Application.Categories.Commands;
using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using CatalogService.Domain.Interfaces;
using MediatR;

namespace CatalogService.Application.Categories.Handlers;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICatalogRepository _repository;
    private readonly IMapper _mapper;

    public CreateCategoryHandler(ICatalogRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category { Name = request.Name };
        var result = await _repository.AddCategoryAsync(category);
        return _mapper.Map<CategoryDto>(result);
    }
}
