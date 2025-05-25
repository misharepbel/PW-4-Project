using MediatR;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Users.Queries;

public record GetAllUsersQuery() : IRequest<List<UserDto>>;
