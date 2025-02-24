using Microsoft.EntityFrameworkCore;
using WebApplicationDemoContext.core.Model;
using WebApplicationDemoContext.DBContext;
using WebApplicationDemoContext.Repositories;

namespace WebApplicationDemoContext.Core.Adapter
{
    public class AdapterUser : IUserRepository
    {
        private readonly AppDBContext _dbContext;

        public AdapterUser(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UpdateUserByID(User user)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (existingUser == null)
            {
                return false;
            }

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Password = user.Password;

            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteUserByID(int id)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {id} not found.");
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public Task<User> GetUserByID(int id)
        {
            return _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByName(string name)
        {
            return await _dbContext.Users.Where(e => e.Name == name).FirstOrDefaultAsync();
        }

    }
}