using Microsoft.AspNetCore.Identity;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.DTOs;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Domain.Abstractions;
using Todo.Identity.Domain.Entities;
using Todo.Identity.Domain.ValueObjects;

namespace Todo.Identity.Application.Commands;

public static class UpdateUser
{
    public record Command : ICommand<Result>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
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

            // Update Value Objects
            var fullName = FullName.Create(request.FirstName, request.LastName);
            var email = Email.Create(request.Email);
            var phoneNumber = request.PhoneNumber is not null ? PhoneNumber.Create(request.PhoneNumber) : null;

            // Check if email is already taken by another user
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null && existingUser.Id != request.UserId)
                throw new EmailAlreadyExistsException($"Email {request.Email} is already taken");

            user.UpdateProfile(fullName, email, phoneNumber);

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new UserInfoUpdateFailedException($"Failed to update user: {errors}");
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new UserInfoDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FullName.FirstName,
                LastName = user.FullName.LastName,
                PhoneNumber = user.PhoneNumber?.Value,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }
    }
}