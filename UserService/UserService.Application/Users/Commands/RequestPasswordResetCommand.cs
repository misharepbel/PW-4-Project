using MediatR;

namespace UserService.Application.Users.Commands;

public record RequestPasswordResetCommand(string Email) : IRequest;
