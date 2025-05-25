using UserService.Domain.Enums;

namespace UserService.Domain.Entities;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public bool IsEmailVerified { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
}
