namespace UserService.Application.Interfaces;

public interface IResetTokenStore
{
    void Store(Guid userId, string token);
    Guid? Take(string token);
}
