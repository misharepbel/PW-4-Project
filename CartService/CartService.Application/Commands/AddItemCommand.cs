using CartService.Application.DTOs;
using MediatR;

namespace CartService.Application.Commands;

public record AddItemCommand(Guid UserId, AddCartItemDto Item) : IRequest;
