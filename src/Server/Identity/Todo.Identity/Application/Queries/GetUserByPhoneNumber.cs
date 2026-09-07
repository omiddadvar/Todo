using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.DTOs;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Application.Queries;

public static class GetUserByPhoneNumber
{
    public record Query : IQuery<Result>
    {
        public string PhoneNumber { get; set; }
    }

    public class Handler(
        UserManager<User> userManager) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            // Clean phone number (remove non-digit characters)
            var cleanPhone = new string(request.PhoneNumber.Where(char.IsDigit).ToArray());

            var user = await userManager.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber.Value == cleanPhone, cancellationToken);

            if (user == null)
                throw new UserNotFoundException($"User with phone number {request.PhoneNumber} not found");

            var roles = await userManager.GetRolesAsync(user);

            return Result.Success(new UserInfoDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FullName.FirstName,
                LastName = user.FullName.LastName,
                PhoneNumber = user.PhoneNumber?.Value,
                Roles = roles.ToList(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            });
        }
    }
}