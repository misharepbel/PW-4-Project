using MediatR;

namespace CartService.Application.Commands;

public record ClearCartCommand(Guid UserId) : IRequest;
