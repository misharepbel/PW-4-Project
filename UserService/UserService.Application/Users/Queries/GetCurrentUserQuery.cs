using MediatR;
using System.Security.Claims;
using UserService.Application.Users.DTOs;

namespace UserService.Application.Users.Queries;

public record GetCurrentUserQuery(ClaimsPrincipal User) : IRequest<UserDto>;
