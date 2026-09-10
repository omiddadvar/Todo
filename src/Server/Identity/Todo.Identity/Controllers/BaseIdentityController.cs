using Microsoft.AspNetCore.Mvc;

namespace Todo.Identity.Controllers;

[ApiController]
public class BaseIdentityController : ControllerBase
{
    protected Guid? GetUserIdFromClaims()
    {
        var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            return userId;
        return null;
    }

}
