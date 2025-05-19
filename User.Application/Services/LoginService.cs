using User.Domain.Exceptions;
namespace User.Application.Services;

public class LoginService : ILoginService
{
    private IJwtTokenService _jwtTokenService;
    public LoginService(IJwtTokenService jwtTokenService)
    {
        _jwtTokenService = jwtTokenService;
    }
    public string Login(string username, string password)
    {
        if (username=="admin" || password=="password")
        {
            var roles = new List<string> { "Client", "Employee", "Administrator" };
            var token = _jwtTokenService.GenerateToken(1, roles);
            return token;
        }
        else
        {
            throw new InvalidCredentialsException();
        }
    }
}
