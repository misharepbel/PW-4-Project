using UserService.Domain.Entities;

namespace UserService.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
