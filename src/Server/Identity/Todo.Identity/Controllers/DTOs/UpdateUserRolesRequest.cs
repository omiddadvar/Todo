namespace Todo.Identity.Controllers.DTOs;

public record UpdateUserRolesRequest
{
    public List<string> Roles { get; init; } = new();
}