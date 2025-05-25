using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Text;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;

namespace UserService.Application.Users.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterUserCommandHandler(IUserRepository repo, IPasswordHasher<User> passwordHasher)
    {
        _repo = repo;
        _passwordHasher = passwordHasher;
    }


    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            IsEmailVerified = false
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _repo.AddAsync(user);

        return user.Id;
    }
}
