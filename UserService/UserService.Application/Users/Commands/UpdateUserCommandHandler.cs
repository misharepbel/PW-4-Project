using MediatR;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Interfaces;
using UserService.Domain.Entities;
using UserService.Domain.Exceptions;

namespace UserService.Application.Users.Commands;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordHasher<User> _hasher;

    public UpdateUserCommandHandler(IUserRepository repo, IPasswordHasher<User> hasher)
    {
        _repo = repo;
        _hasher = hasher;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repo.GetByIdAsync(request.Id) ?? throw new UserNotFoundException(request.Id.ToString());

        if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
        {
            var existing = await _repo.GetByEmailAsync(request.Email);
            if (existing is not null && existing.Id != user.Id)
            {
                throw new Exception($"Email '{request.Email}' is already taken.");
            }
            user.Email = request.Email;
        }

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            user.Username = request.Username;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            user.PasswordHash = _hasher.HashPassword(user, request.Password);
        }

        await _repo.UpdateAsync(user);
    }
}
