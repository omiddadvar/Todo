using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Todo.Identity.Application.Abstractions;
using Todo.Identity.Application.DTOs;
using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Application.Queries;

public static class GetAllUsers
{
    public record Query : IQuery<Result>
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public string? PhoneSearch { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
    }

    public class Handler(
        UserManager<User> userManager) : IQueryHandler<Query, Result>
    {
        public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = userManager.Users.AsQueryable();

            // Filter by active status
            if (request.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActive.Value);
            }

            // Search by name (FirstName or LastName)
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = ApplySearchByName(query, request.SearchTerm);
            }

            // Search by phone number
            if (!string.IsNullOrWhiteSpace(request.PhoneSearch))
            {
                query = ApplySearchByPhone(query, request.PhoneSearch);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = ApplySorting(query, request.SortBy, request.SortDescending);

            // Apply pagination
            var users = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            // Get roles for each user
            var userInfos = new List<UserInfoDTO>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userInfos.Add(new UserInfoDTO
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

            return Result.Success(new PaginatedResultDTO<UserInfoDTO>
            {
                Items = userInfos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            });
        }
        #region Private Helper Methods
        private IQueryable<User> ApplySearchByName(IQueryable<User> query, string searchTerm)
        {
            searchTerm = searchTerm.ToLower().Trim();
            return query.Where(u =>
                u.FullName.FirstName.ToLower().Contains(searchTerm) ||
                u.FullName.LastName.ToLower().Contains(searchTerm) ||
                (u.FullName.FirstName + " " + u.FullName.LastName).ToLower().Contains(searchTerm)
            );
        }

        private IQueryable<User> ApplySearchByPhone(IQueryable<User> query, string phoneNumber)
        {
            var cleanPhone = new string(phoneNumber.Where(char.IsDigit).ToArray());
            return query.Where(u =>
                u.PhoneNumber != null &&
                u.PhoneNumber.Value.Contains(cleanPhone)
            );
        }
        private IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool descending)
        {
            return sortBy?.ToLower() switch
            {
                "email" => descending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email),
                "firstname" => descending
                    ? query.OrderByDescending(u => u.FullName.FirstName)
                    : query.OrderBy(u => u.FullName.FirstName),
                "lastname" => descending
                    ? query.OrderByDescending(u => u.FullName.LastName)
                    : query.OrderBy(u => u.FullName.LastName),
                "isactive" => descending
                    ? query.OrderByDescending(u => u.IsActive)
                    : query.OrderBy(u => u.IsActive),
                "createdat" => descending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt),
                "updatedat" => descending
                    ? query.OrderByDescending(u => u.UpdatedAt)
                    : query.OrderBy(u => u.UpdatedAt),
                _ => descending
                    ? query.OrderByDescending(u => u.CreatedAt)
                    : query.OrderBy(u => u.CreatedAt)
            };
        }
        #endregion
    }
}