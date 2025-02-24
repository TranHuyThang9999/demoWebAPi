using WebApplicationDemoContext.core.Model;

namespace WebApplicationDemoContext.Repositories;

public interface IUserRepository
{
    public Task <User>AddUser(User user);
    public Task<User> UpdateUserByID(User user);
    public Task<User> DeleteUserByID(int id);
    public Task<User> GetUserByID(int id);
    public Task<User>GetUserByName(string name);
}