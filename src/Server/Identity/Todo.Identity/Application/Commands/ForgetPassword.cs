using MassTransit;
using Microsoft.AspNetCore.Identity;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Constants;
using Todo.Identity.Domain.Entities;
using Todo.Identity.Messages.Events;

namespace Todo.Identity.Application.Commands;

public static class ForgetPassword
{
    public record Command : ICommand<Result>
    {
        public string Email { get; set; }
        public string? ResetUrl { get; set; }
        public string? ClientIP { get; set; }
    }

    public class Handler(
        UserManager<User> userManager,
        IPublishEndpoint publishEndpoint,
        IConfiguration configuration) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UserNotFoundException($"User with email {request.Email} not found");

            if (!user.IsActive)
                throw new AccountDeactivatedException("Account is deactivated");

            // Generate password reset token
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            // Create reset link
            var baseUrl = request.ResetUrl ?? configuration[ConfigKeyword.Appsetting.BaseUrl];
            var resetLink = $"{baseUrl}/reset-password?email={user.Email}&token={Uri.EscapeDataString(resetToken)}";

            // Publish event
            await publishEndpoint.Publish(new UserPasswordResetRequestedEvent
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = $"{user.FullName.FirstName} {user.FullName.LastName}",
                ResetLink = resetLink,
                ResetToken = resetToken,
                RequestedAt = DateTime.UtcNow,
                ClientIP = request.ClientIP
            }, cancellationToken);

            return Result.Success(new
            {
                Message = "Password reset link has been sent to your email",
                Email = user.Email,
                ResetLinkSent = true
            });
        }
    }
}