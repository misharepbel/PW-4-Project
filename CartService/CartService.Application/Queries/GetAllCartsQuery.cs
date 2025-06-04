using CartService.Application.DTOs;
using MediatR;

namespace CartService.Application.Queries;

public record GetAllCartsQuery() : IRequest<List<CartDto>>;
