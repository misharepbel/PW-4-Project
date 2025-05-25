using MediatR;
using UserService.Application.Users.DTOs;
using UserService.Domain.Interfaces;

namespace UserService.Application.Users.Queries;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
{
    private readonly IUserRepository _repo;

    public GetUserByIdQueryHandler(IUserRepository repo)
    {
        _repo = repo;
    }

    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _repo.GetByIdAsync(request.Id)
            ?? throw new Exception("User not found");

        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role.ToString()
        };
    }
}
