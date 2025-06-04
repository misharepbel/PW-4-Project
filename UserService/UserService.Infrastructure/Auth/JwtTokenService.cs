using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly RsaSecurityKey _key;

    public JwtTokenService(IConfiguration config)
    {
        var privateKeyPem = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY");

        if (string.IsNullOrWhiteSpace(privateKeyPem))
            throw new InvalidOperationException("JWT_PRIVATE_KEY is not set.");

        privateKeyPem = privateKeyPem.Replace("\\n", "\n");

        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem.ToCharArray());

        _key = new RsaSecurityKey(rsa);

    }

    public string GenerateToken(User user)
    {
        var creds = new SigningCredentials(_key, SecurityAlgorithms.RsaSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),  
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "UserService",
            audience: "TeaShop",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60), // change later
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
