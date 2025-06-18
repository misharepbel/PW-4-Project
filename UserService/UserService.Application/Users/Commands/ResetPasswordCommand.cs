using MediatR;

namespace UserService.Application.Users.Commands;

public record ResetPasswordCommand(string Token, string NewPassword) : IRequest;
