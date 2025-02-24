using Microsoft.EntityFrameworkCore;
using WebApplicationDemoContext.core.Model;
using WebApplicationDemoContext.DBContext;
using WebApplicationDemoContext.Repositories;

namespace WebApplicationDemoContext.Repository
{
    public class AdapterUser : IUserRepository
    {
        private readonly AppDBContext _dbContext;
        public async Task<User> AddUser(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public Task<User> UpdateUserByID(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> DeleteUserByID(int id)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByID(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByName(string name)
        {
            return await _dbContext.Users.Where(e =>e.Name == name).FirstOrDefaultAsync();
        }
    }
}