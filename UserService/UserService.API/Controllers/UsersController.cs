using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Users.Commands;
using UserService.Application.Users.Queries;

namespace UserService.API.Controllers;

[ApiController]
[Route("")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public Task<ActionResult<Guid>> HealthCheck()
    => Task.FromResult<ActionResult<Guid>>(Ok("UserService is listening..."));

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return Ok(new { userId });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(new { token });
    }

    [HttpGet("admin-test")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(string), 200)]
    public IActionResult OnlyForAdmins()
    {
        return Ok(new
        {
            message = "Welcome, Admin!",
            time = DateTime.UtcNow
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var dto = await _mediator.Send(new GetCurrentUserQuery(User));
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetUserByIdQuery(id));
        return Ok(dto);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _mediator.Send(new GetAllUsersQuery());
        return Ok(list);
    }
}
