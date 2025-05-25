using MediatR;

namespace UserService.Application.Users.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<string>;
