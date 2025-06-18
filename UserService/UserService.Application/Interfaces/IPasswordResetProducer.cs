using UserService.Application.DTOs;

namespace UserService.Application.Interfaces;

public interface IPasswordResetProducer
{
    Task PublishAsync(PasswordResetEvent evt, CancellationToken ct = default);
}
