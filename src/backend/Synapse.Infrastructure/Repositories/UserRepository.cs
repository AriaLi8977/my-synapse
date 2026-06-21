using Synapse.Application.Interfaces;
using Synapse.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Synapse.Infrastructure.Data;

public class UserRepository: IUserRepository{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _db.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<User> AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _db.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
    }
}