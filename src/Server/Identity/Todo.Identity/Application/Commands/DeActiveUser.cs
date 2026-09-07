using Microsoft.AspNetCore.Identity;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Application.Commands;

public static class DeActiveUser
{
    public record Command : ICommand<Result>
    {
        public Guid UserId { get; set; }
        public bool IsActive { get; set; }
        public string? Reason { get; set; }
    }

    public class Handler(
        UserManager<User> userManager,
        IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new UserNotFoundException($"User with ID {request.UserId} not found");

            if (request.IsActive)
            {
                user.Activate();
            }
            else
            {
                user.Deactivate();
                user.RevokeRefreshToken();
            }

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new UserInfoUpdateFailedException($"Failed to update user status: {errors}");
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new
            {
                UserId = user.Id,
                IsActive = user.IsActive,
                Message = request.IsActive ? "User activated successfully" : "User deactivated successfully"
            });
        }
    }
}