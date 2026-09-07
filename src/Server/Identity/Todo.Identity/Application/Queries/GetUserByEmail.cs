using Microsoft.AspNetCore.Identity;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.DTOs;
using Todo.Identity.Application.Exceptions;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Application.Queries;

public static class GetUserByEmail
{
    public record Query : IQuery<Result>
    {
        public string Email { get; set; }
    }

    public class Handler(
        UserManager<User> userManager) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new UserNotFoundException($"User with email {request.Email} not found");

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