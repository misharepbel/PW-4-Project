using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Users.Commands;
using UserService.Application.Users.Queries;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerOperation(Summary = "Health check", Description = "Access: Public")]
    public Task<ActionResult<Guid>> HealthCheck()
    => Task.FromResult<ActionResult<Guid>>(Ok("UserService is listening..."));

    [HttpPost("register")]
    [SwaggerOperation(Summary = "Register a new user", Description = "Access: Public")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return Ok(new { userId });
    }

    [HttpPost("login")]
    [SwaggerOperation(Summary = "Log in and obtain JWT", Description = "Access: Public")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        var token = await _mediator.Send(command);
        return Ok(new { token });
    }

    [HttpPost("password-reset")]
    [SwaggerOperation(Summary = "Request password reset", Description = "Access: Public")]
    public async Task<IActionResult> RequestPasswordReset([FromBody] RequestPasswordResetCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("reset-password")]
    [SwaggerOperation(Summary = "Reset password with token", Description = "Access: Public")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("admin-test")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(string), 200)]
    [SwaggerOperation(Summary = "Example endpoint for admins", Description = "Access: Admin only")]
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
    [SwaggerOperation(Summary = "Get current user's details", Description = "Access: User & Admin")]
    public async Task<IActionResult> Me()
    {
        var dto = await _mediator.Send(new GetCurrentUserQuery(User));
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Get user by id", Description = "Access: Admin only")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var dto = await _mediator.Send(new GetUserByIdQuery(id));
        return Ok(dto);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "List all users", Description = "Access: Admin only")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _mediator.Send(new GetAllUsersQuery());
        return Ok(list);
    }
}
