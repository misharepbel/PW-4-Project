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
    private readonly IResetTokenStore _tokens;

    public RequestPasswordResetCommandHandler(IUserRepository repo, IPasswordResetProducer producer, IResetTokenStore tokens)
    {
        _repo = repo;
        _producer = producer;
        _tokens = tokens;
    }

    public async Task Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var user = await _repo.GetByEmailAsync(request.Email)
            ?? throw new Exception("User not found");

        var token = Guid.NewGuid().ToString();
        _tokens.Store(user.Id, token);
        await _producer.PublishAsync(new PasswordResetEvent
        {
            UserId = user.Id,
            Email = user.Email,
            ResetToken = token
        }, cancellationToken);
    }
}
