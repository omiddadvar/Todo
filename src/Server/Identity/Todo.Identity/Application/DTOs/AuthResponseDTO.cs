namespace Todo.Identity.Application.DTOs;

public record AuthResponseDTO
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiryTime { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
