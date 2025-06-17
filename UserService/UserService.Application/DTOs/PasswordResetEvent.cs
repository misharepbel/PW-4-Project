namespace UserService.Application.DTOs;

public class PasswordResetEvent
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string ResetToken { get; set; } = string.Empty;
}
