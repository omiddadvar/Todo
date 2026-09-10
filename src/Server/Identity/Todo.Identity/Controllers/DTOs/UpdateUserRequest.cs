namespace Todo.Identity.Controllers.DTOs;

public record UpdateUserRequest
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string? PhoneNumber { get; init; }
}
