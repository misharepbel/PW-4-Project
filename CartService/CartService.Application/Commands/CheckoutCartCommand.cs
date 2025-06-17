using CartService.Application.DTOs;
using MediatR;

namespace CartService.Application.Commands;

public record CheckoutCartCommand(Guid UserId, CheckoutInfoDto Info) : IRequest;
