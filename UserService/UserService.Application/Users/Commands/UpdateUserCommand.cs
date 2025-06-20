using MediatR;

namespace UserService.Application.Users.Commands;

public record UpdateUserCommand(Guid Id, string? Email, string? Username) : IRequest;
