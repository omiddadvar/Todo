namespace Todo.Identity.Controllers.DTOs;

public record LoginRequest
{
    public string Email { get; init; } = default!;
    public string Password { get; init; } = default!;
    public bool RememberMe { get; init; }
}
