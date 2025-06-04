using CartService.Application.DTOs;
using MediatR;

namespace CartService.Application.Queries;

public record GetCartQuery(Guid UserId) : IRequest<CartDto?>;
