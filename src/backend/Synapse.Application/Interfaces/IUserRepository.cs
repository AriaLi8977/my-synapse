using Synapse.Domain.Entities;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsAsync(string email);
    Task<User> AddAsync(User user);
    Task<User> UpdateAsync(User user);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}