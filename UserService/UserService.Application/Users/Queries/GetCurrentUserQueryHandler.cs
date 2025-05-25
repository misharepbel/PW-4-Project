using MediatR;
using System.Security.Claims;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Users.Queries;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserDto>
{
    public Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = request.User;

        var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = user.FindFirstValue(ClaimTypes.Email);
        var username = user.FindFirstValue(ClaimTypes.Name);
        var role = user.FindFirstValue(ClaimTypes.Role);

        return Task.FromResult(new UserDto
        {
            Id = Guid.Parse(id!),
            Email = email!,
            Username = username!,
            Role = role!
        });
    }
}
