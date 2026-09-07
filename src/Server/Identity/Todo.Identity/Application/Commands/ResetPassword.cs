using Microsoft.AspNetCore.Identity;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Application.Commands;

public static class ResetPassword
{
    public record Command : ICommand<Result>
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class Handler(
        UserManager<User> userManager,
        IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new ConfirmPasswordNotCorrectException("Passwords do not match");

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UserNotFoundException($"User with email {request.Email} not found");

            if (!user.IsActive)
                throw new AccountDeactivatedException("Account is deactivated");

            var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new PasswordResetFailedException($"Failed to reset password: {errors}");
            }

            // Revoke all refresh tokens on password reset for security
            user.RevokeRefreshToken();
            await userManager.UpdateAsync(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new
            {
                Message = "Password has been reset successfully",
                Email = user.Email
            });
        }
    }
}