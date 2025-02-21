using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using WebApplicationDemoContext.DBContext;
using WebApplicationDemoContext.model;
using WebApplicationDemoContext.Repositories;

namespace WebApplicationDemoContext.Repository;

public class UserRepository : IUser
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext dbContext, ILogger<UserRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<User> AddUser(User user)
    {
        try
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
        catch (DbException e)
        {
            _logger.LogError(e, "Error when adding user: {Message}", e.Message);
            throw;
        }
    }

    public async Task<User> UpdateUserByID(User user)
    {
        try
        {
            var existingUser = await _dbContext.Users.FindAsync(user.Id);
            if (existingUser == null) return null!;

            _dbContext.Entry(existingUser).CurrentValues.SetValues(user);
            await _dbContext.SaveChangesAsync();
            return existingUser;
        }
        catch (DbException e)
        {
            _logger.LogError(e, "Error when updating user: {Message}", e.Message);
            throw;
        }
    }

    public async Task<User> DeleteUserByID(int id)
    {
        try
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return null!;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
        catch (DbException e)
        {
            _logger.LogError(e, "Error when deleting user: {Message}", e.Message);
            throw;
        }
    }

    public async Task<User> GetUserByID(int id)
    {
        try
        {
            return await _dbContext.Users.FindAsync(id) ?? throw new InvalidOperationException();
        }
        catch (DbException e)
        {
            _logger.LogError(e, "Error when retrieving user by ID: {Message}", e.Message);
            throw;
        }
    }

    public async Task<User> GetUserByName(string name)
    {
        try
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Name == name) ?? throw new InvalidOperationException();
        }
        catch (DbException e)
        {
            _logger.LogError(e, "Error when retrieving user by name: {Message}", e.Message);
            throw;
        }
    }
}
