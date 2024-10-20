using Application.Commands.Login.Create;
using Application.Commands.Logout.Create;
using Application.Commands.User.Create;
using Application.Notification;
using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/account")]
public class AccountsController : MainController
{
    readonly IMediator _mediator;

    public AccountsController(INotificationError notificationError, IMediator mediator) : base(notificationError)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Authenticates a user based on the provided email and password.
    /// </summary>
    /// <param name="createLoginCommandRequest">Object containing the user's email and password.</param>
    /// <returns>An object containing the access token and user information, or a problem details response in case of an error.</returns>
    /// <response code="200">Returns the access token and user information.</response>
    /// <response code="400">If the request model is invalid.</response>
    /// <response code="500">If a server error occurs.</response>
    [AllowAnonymous]
    [ProducesResponseType(typeof(CreateLoginCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("login")]
    public async Task<ActionResult> Authenticate([FromBody] CreateLoginCommandRequest createLoginCommandRequest)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var isAuthenticate = await _mediator.Send(createLoginCommandRequest);

        if (!ValidOperation()) return CustomResponse();

        return Ok(isAuthenticate);
    }

    /// <summary>
    /// Registers a new user based on the provided information.
    /// </summary>
    /// <param name="createUserCommandRequest">Object containing the information of the user to be registered.</param>
    /// <returns>A success status or a problem details object in case of an error.</returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">If the request model is invalid.</response>
    /// <response code="500">If a server error occurs.</response>
    [ProducesResponseType(typeof(CreateUserCommandRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("register-user")]
    public async Task<ActionResult> RegisterUsuer([FromBody] CreateUserCommandRequest createUserCommandRequest)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        await _mediator.Send(createUserCommandRequest);

        if (!ValidOperation()) return CustomResponse();

        return Ok();
    }

    /// <summary>
    /// Performs logout for the authenticated user.
    /// </summary>
    /// <returns>A success status or a problem details object in case of an error.</returns>
    /// <response code="200">Logout successful.</response>
    /// <response code="400">If the request model is invalid.</response>
    /// <response code="500">If a server error occurs.</response>
    [Authorize]
    [ProducesResponseType(typeof(CreateLogoutCommandRequest), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        await _mediator.Send(new CreateLogoutCommandRequest());

        if (!ValidOperation()) return CustomResponse();

        return Ok();
    }
}
