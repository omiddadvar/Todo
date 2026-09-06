using Todo.Identity.Domain.Entities;

namespace Todo.Identity.Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByRefreshTokenAsync(string refreshToken);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<IEnumerable<User>> GetUsersByRoleAsync(string roleName);
}
