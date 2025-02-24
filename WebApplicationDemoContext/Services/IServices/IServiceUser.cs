using WebApplicationDemoContext.Common;
using WebApplicationDemoContext.core.Model;
using WebApplicationDemoContext.DTO;

namespace WebApplicationDemoContext.Services.IServices;

public interface IServiceUser
{
    Task<Result<User>> AddUser(RequestUserCreate request);
    Task<Result<UserLoginResponse>> Login(RequestUserLogin request);
    Task<Result<User>> GetUserByUserID(int userID);
}