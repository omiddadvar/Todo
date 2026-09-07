using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Domain.Entities;
using Todo.Identity.Infrastructure.Exceptions;

namespace Todo.Identity.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly SymmetricSecurityKey _securityKey;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;

        var secretKey = _configuration["JwtSettings:SecretKey"]
            ?? throw new InvalidOperationException("JWT Secret Key is not configured");

        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false, // We want to validate expired tokens
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JwtSettings:Issuer"],
            ValidAudience = _configuration["JwtSettings:Audience"],
            IssuerSigningKey = _securityKey,
            ClockSkew = TimeSpan.Zero
        };
    }

    public (string AccessToken, DateTime ExpiryTime) GenerateAccessToken(User user, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FullName.FirstName} {user.FullName.LastName}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("firstName", user.FullName.FirstName),
            new Claim("lastName", user.FullName.LastName),
            new Claim("email", user.Email),
            new Claim("userId", user.Id.ToString())
        };

        // Add roles as claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var expiry = DateTime.UtcNow.AddMinutes(
            Convert.ToDouble(_configuration["JwtSettings:AccessTokenExpiryMinutes"] ?? "15"));

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256)
        );

        return (new JwtSecurityTokenHandler().WriteToken(token), expiry);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal ValidateExpiredToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Validate token without checking expiration
            var principal = tokenHandler.ValidateToken(
                token,
                _tokenValidationParameters,
                out SecurityToken validatedToken);

            // Ensure it's a valid JWT
            if (validatedToken is not JwtSecurityToken jwtToken)
                throw new InvalidTokenException("Invalid token");

            // Ensure it uses the correct algorithm
            if (!jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
                throw new InvalidTokenException("Invalid token algorithm");

            return principal;
        }
        catch (Exception ex)
        {
            throw new InvalidTokenException($"Token validation failed: {ex.Message}");
        }
    }

    public Guid GetUserIdFromExpiredToken(string token)
    {
        var principal = ValidateExpiredToken(token);
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
            ?? principal.FindFirst("userId");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new InvalidTokenUserIdException("Invalid user ID in token");

        return userId;
    }

    /// <summary>
    /// Additional helper methods for token management
    /// </summary>
    public bool IsTokenExpired(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }

    public DateTime GetTokenExpiryDate(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.ValidTo;
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    public Dictionary<string, string> DecodeToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            return jwtToken.Claims.ToDictionary(
                claim => claim.Type,
                claim => claim.Value
            );
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}