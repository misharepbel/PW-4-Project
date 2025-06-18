using MediatR;
using System;
using UserService.Application.DTOs;
using UserService.Application.Interfaces;
using UserService.Domain.Interfaces;

namespace UserService.Application.Users.Commands;

public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand>
{
    private readonly IUserRepository _repo;
    private readonly IPasswordResetProducer _producer;

    public RequestPasswordResetCommandHandler(IUserRepository repo, IPasswordResetProducer producer)
    {
        _repo = repo;
        _producer = producer;
    }

    public async Task Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var user = await _repo.GetByEmailAsync(request.Email)
            ?? throw new Exception("User not found");

        var token = Guid.NewGuid().ToString();
        await _producer.PublishAsync(new PasswordResetEvent
        {
            UserId = user.Id,
            Email = user.Email,
            ResetToken = token
        }, cancellationToken);
    }
}
