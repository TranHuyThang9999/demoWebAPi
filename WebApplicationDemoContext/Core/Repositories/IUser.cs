using WebApplicationDemoContext.core.Model;

namespace WebApplicationDemoContext.Core.Repositories;

public interface IUserRepository
{
    public Task <User>CreateUser(User user);
    public Task<bool> UpdateUserByID(User user);
    public Task<bool> DeleteUserByID(int id);
    public Task<User> GetUserByID(int id);
    public Task<User>GetUserByName(string name);
}