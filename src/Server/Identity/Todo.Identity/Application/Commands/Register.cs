using Microsoft.AspNetCore.Identity;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.DTOs;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Constants;
using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Constants;
using Todo.Identity.Domain.Entities;
using Todo.Identity.Domain.ValueObjects;

namespace Todo.Identity.Application.Commands;

public static class Register
{
    public record Command : ICommand<Result>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool RememberMe { get; set; } = false;
    }
    public class Handler(
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        ITokenService tokenService,
        IConfiguration configuration) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            try
            {
                if (!request.Password.Equals(request.ConfirmPassword))
                {
                    throw new ConfirmPasswordNotCorrectException($"ConfirmPassword and Password don't not match");
                }

                await unitOfWork.BeginTransactionAsync(cancellationToken);

                // Create Value Objects
                var email = Email.Create(request.Email);
                var fullName = FullName.Create(request.FirstName, request.LastName);
                var phoneNumber = request.PhoneNumber is not null ? PhoneNumber.Create(request.PhoneNumber) : null;

                // Create User
                var user = User.Create(Guid.NewGuid(), fullName, email, phoneNumber);

                var result = await userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new RegistrationFailedException($"Registration failed: {errors}");
                }

                // Assign default role
                await userManager.AddToRoleAsync(user, RoleName.User);

                // Generate tokens
                var roles = await userManager.GetRolesAsync(user);
                var (accessToken, accessTokenExpiry) = tokenService.GenerateAccessToken(user, roles.ToList());

                DateTime? refreshTokenExpiryTime = null;
                string? refreshToken = null;

                if (request.RememberMe)
                {
                    refreshToken = tokenService.GenerateRefreshToken();
                    int refreshTokenExpiryinDays = configuration.GetValue(ConfigKeyword.Token.RefreshTokenExpireInDays, 7);
                    refreshTokenExpiryTime = DateTime.UtcNow.AddDays(refreshTokenExpiryinDays);
                    user.SetRefreshToken(refreshToken, refreshTokenExpiryTime.Value);
                    await userManager.UpdateAsync(user);
                }

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result.Success(
                    new AuthResponseDTO
                    {
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        AccessTokenExpiryTime = accessTokenExpiry,
                        RefreshTokenExpiryTime = refreshTokenExpiryTime
                    }
                );
            }
            catch
            {
                await unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }

}