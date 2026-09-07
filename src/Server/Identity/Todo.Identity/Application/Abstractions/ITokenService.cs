using System.Security.Claims;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Application.Abstractions;

public interface ITokenService
{
    (string AccessToken, DateTime ExpiryTime) GenerateAccessToken(User user, List<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal ValidateExpiredToken(string token);
    Guid GetUserIdFromExpiredToken(string token);
}

