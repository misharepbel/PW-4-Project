using UserService.Application.DTOs;

namespace UserService.Application.Interfaces;

public interface IUserRegisteredProducer
{
    Task PublishAsync(UserRegisteredEvent evt, CancellationToken ct = default);
}
