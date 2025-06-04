using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;

namespace CartService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CartItem, CartItemDto>().ReverseMap();
        CreateMap<Cart, CartDto>();
    }
}
