using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Text;
using UserService.Domain.Entities;
using UserService.Domain.Interfaces;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;

namespace UserService.Application.Users.Commands;

public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUserRegisteredProducer _producer;

    public RegisterUserCommandHandler(IUserRepository repo, IPasswordHasher<User> passwordHasher, IUserRegisteredProducer producer)
    {
        _repo = repo;
        _passwordHasher = passwordHasher;
        _producer = producer;
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
        await _producer.PublishAsync(new UserRegisteredEvent
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.Username
        }, cancellationToken);

        return user.Id;
    }
}
