using MediatR;
using UserService.Application.Users.DTOs;
using UserService.Domain.Interfaces;

namespace UserService.Application.Users.Queries;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserDto>>
{
    private readonly IUserRepository _repo;

    public GetAllUsersQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _repo.GetAllAsync();

        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Email = u.Email,
            Username = u.Username,
            Role = u.Role.ToString()
        }).ToList();
    }
}
