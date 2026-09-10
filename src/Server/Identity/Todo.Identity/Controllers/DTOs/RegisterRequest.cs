namespace Todo.Identity.Controllers.DTOs;

public record RegisterRequest
{
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public string ConfirmPassword { get; init; } = default!;
    public string? PhoneNumber { get; init; }
    public bool RememberMe { get; init; }
}
