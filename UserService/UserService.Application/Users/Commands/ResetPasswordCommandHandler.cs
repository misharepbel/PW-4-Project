using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Domain.Exceptions;

namespace UserService.Application.Users.Commands;

public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IResetTokenStore _tokens;

    public ResetPasswordCommandHandler(IUserRepository repo, IPasswordHasher<User> hasher, IResetTokenStore tokens)
    {
        _repo = repo;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _tokens.Take(request.Token) ?? throw new Exception("Invalid token");

        var user = await _repo.GetByIdAsync(userId) ?? throw new UserNotFoundException(userId.ToString());

        user.PasswordHash = _hasher.HashPassword(user, request.NewPassword);

        await _repo.UpdateAsync(user);
    }
}
