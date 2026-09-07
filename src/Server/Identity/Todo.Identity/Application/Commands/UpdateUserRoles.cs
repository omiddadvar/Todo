using Microsoft.AspNetCore.Identity;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.DTOs;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Application.Commands;

public static class UpdateUserRoles
{
    public record Command : ICommand<Result>
    {
        public Guid UserId { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class Handler(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IUnitOfWork unitOfWork) : ICommandHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                throw new UserNotFoundException($"User with ID {request.UserId} not found");

            // Validate roles exist
            var invalidRoles = new List<string>();
            foreach (var roleName in request.Roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    invalidRoles.Add(roleName);
            }

            if (invalidRoles.Any())
                throw new InvalidOperationException($"Invalid roles: {string.Join(", ", invalidRoles)}");

            // Get current roles
            var currentRoles = await userManager.GetRolesAsync(user);

            // Remove all current roles
            var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join(", ", removeResult.Errors.Select(e => e.Description));
                throw new UserInfoUpdateFailedException($"Failed to remove roles: {errors}");
            }

            // Add new roles
            if (request.Roles.Any())
            {
                var addResult = await userManager.AddToRolesAsync(user, request.Roles);
                if (!addResult.Succeeded)
                {
                    var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    throw new UserInfoUpdateFailedException($"Failed to add roles: {errors}");
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedRoles = await userManager.GetRolesAsync(user);
            return Result.Success(new UserRolesDTO
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = updatedRoles.ToList()
            });
        }
    }
}