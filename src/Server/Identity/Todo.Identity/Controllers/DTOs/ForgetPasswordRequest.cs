namespace Todo.Identity.Controllers.DTOs;

public record ForgetPasswordRequest
{
    public string Email { get; init; } = default!;
    public string? ResetUrl { get; init; }
}
