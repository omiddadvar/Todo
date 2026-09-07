namespace Todo.Identity.Application.DTOs;

public record UserRolesDTO
{
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public List<string> Roles { get; init; } = new();
}
