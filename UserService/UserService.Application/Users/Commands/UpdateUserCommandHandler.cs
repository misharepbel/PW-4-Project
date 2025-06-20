using MediatR;
using UserService.Domain.Interfaces;
using UserService.Domain.Exceptions;

namespace UserService.Application.Users.Commands;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _repo;

    public UpdateUserCommandHandler(IUserRepository repo)
    {
        _repo = repo;
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

        await _repo.UpdateAsync(user);
    }
}
