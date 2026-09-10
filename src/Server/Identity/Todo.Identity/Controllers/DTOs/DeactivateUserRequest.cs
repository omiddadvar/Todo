namespace Todo.Identity.Controllers.DTOs;

public record DeactivateUserRequest
{
    public bool IsActive { get; init; }
    public string? Reason { get; init; }
}