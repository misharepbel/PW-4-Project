using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CatalogService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

        CreateMap<Category, CategoryDto>();
    }
}
