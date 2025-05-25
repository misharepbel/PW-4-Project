using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Users.Commands;

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, string>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IJwtTokenService _jwt;

    public LoginUserCommandHandler(IUserRepository repo, IPasswordHasher<User> hasher, IJwtTokenService jwt)
    {
        _repo = repo;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repo.GetByEmailAsync(request.Email)
                   ?? throw new Exception("Invalid email or password");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new Exception("Invalid email or password");

        return _jwt.GenerateToken(user);
    }
}
