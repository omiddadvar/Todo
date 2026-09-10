namespace Todo.Identity.Controllers.DTOs;

public record ResetPasswordRequest
{
    public string Email { get; init; } = default!;
    public string Token { get; init; } = default!;
    public string NewPassword { get; init; } = default!;
    public string ConfirmPassword { get; init; } = default!;
}