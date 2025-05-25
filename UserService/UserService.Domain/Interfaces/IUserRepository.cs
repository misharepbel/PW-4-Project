using UserService.Domain.Entities;

namespace UserService.Domain.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(Guid id);
    Task<List<User>> GetAllAsync();
}
