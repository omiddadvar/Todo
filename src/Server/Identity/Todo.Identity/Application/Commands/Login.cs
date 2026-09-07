using Microsoft.AspNetCore.Identity;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.Constants;
using Todo.Identity.Application.DTOs;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Application.Commands;

public static class Login
{
    public record Command : ICommand<Result>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class Handler(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        ITokenService tokenService,
        IConfiguration configuration) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new InvalidCredentialsException("Invalid email or password");

            if (!user.IsActive)
                throw new AccountDeactivatedException("Account is deactivated");

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new InvalidCredentialsException("Invalid email or password");

            var roles = await userManager.GetRolesAsync(user);
            var (accessToken, accessTokenExpiry) = tokenService.GenerateAccessToken(user, roles.ToList());

            string? refreshToken = null;
            DateTime? refreshTokenExpiry = null;

            if (request.RememberMe)
            {
                refreshToken = tokenService.GenerateRefreshToken();
                int refreshTokenExpiryInDays = configuration.GetValue(ConfigKeyword.Token.RefreshTokenExpireInDays, 7);
                refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpiryInDays);
                user.SetRefreshToken(refreshToken, refreshTokenExpiry.Value);
                await userManager.UpdateAsync(user);
            }

            return Result.Success(
                new AuthResponseDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiryTime = accessTokenExpiry,
                    RefreshTokenExpiryTime = refreshTokenExpiry
                }
            );
        }
    }
}