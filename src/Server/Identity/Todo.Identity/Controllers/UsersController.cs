using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Todo.Identity.Application.Commands;
using Todo.Identity.Application.DTOs;
using Todo.Identity.Application.Queries;
using Todo.Identity.Controllers.DTOs;
using Todo.Identity.Domain.Constants;

namespace Todo.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseIdentityController
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all users with pagination, search, and filtering.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = nameof(RoleName.Admin))]
    [ProducesResponseType(typeof(PaginatedResultDTO<UserInfoDTO>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers([FromQuery] GetUsersRequest request)
    {
        var query = new GetAllUsers.Query
        {
            SearchTerm = request.SearchTerm,
            IsActive = request.IsActive,
            PhoneSearch = request.PhoneSearch,
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get a user by their ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserInfoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        var query = new GetUserById.Query { UserId = id };
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Get a user by email address.
    /// </summary>
    [HttpGet("by-email")]
    [Authorize(Roles = nameof(RoleName.Admin))]
    [ProducesResponseType(typeof(UserInfoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        var query = new GetUserByEmail.Query { Email = email };
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Get a user by phone number.
    /// </summary>
    [HttpGet("by-phone")]
    [Authorize(Roles = nameof(RoleName.Admin))]
    [ProducesResponseType(typeof(UserInfoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserByPhoneNumber([FromQuery] string phoneNumber)
    {
        var query = new GetUserByPhoneNumber.Query { PhoneNumber = phoneNumber };
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Get roles for a specific user.
    /// </summary>
    [HttpGet("{id:guid}/roles")]
    [Authorize(Roles = nameof(RoleName.Admin))]
    [ProducesResponseType(typeof(UserRolesDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserRoles(Guid id)
    {
        var query = new GetUserRoles.Query { UserId = id };
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Update the current user's profile. Users can update themselves; admins can update any user.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserInfoDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request)
    {
        var currentUserId = GetUserIdFromClaims();
        if (currentUserId == null)
            return Unauthorized();

        // Users can only update their own profile unless they are Admin
        if (currentUserId.Value != id && !User.IsInRole(nameof(RoleName.Admin)))
            return Forbid();

        var command = new UpdateUser.Command
        {
            UserId = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Update roles for a user. Admin only.
    /// </summary>
    [HttpPut("{id:guid}/roles")]
    [Authorize(Roles = nameof(RoleName.Admin))]
    [ProducesResponseType(typeof(UserRolesDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateUserRoles(
        Guid id,
        [FromBody] UpdateUserRolesRequest request)
    {
        var command = new UpdateUserRoles.Command
        {
            UserId = id,
            Roles = request.Roles
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Activate or deactivate a user. Admin only.
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = nameof(RoleName.Admin))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeactivateUser(
        Guid id,
        [FromBody] DeactivateUserRequest request)
    {
        var command = new DeActiveUser.Command
        {
            UserId = id,
            IsActive = request.IsActive,
            Reason = request.Reason
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(result);

        return Ok(result);
    }
}
