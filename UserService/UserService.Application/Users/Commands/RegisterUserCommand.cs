using MediatR;

namespace UserService.Application.Users.Commands
{
    public record RegisterUserCommand(string Username, string Email, string Password) : IRequest<Guid>;
}