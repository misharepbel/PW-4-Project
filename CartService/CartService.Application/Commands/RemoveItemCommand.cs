using MediatR;

namespace CartService.Application.Commands;

public record RemoveItemCommand(Guid UserId, Guid ProductId) : IRequest;
