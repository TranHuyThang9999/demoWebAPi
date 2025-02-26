using WebApplicationDemoContext.Common;
using WebApplicationDemoContext.core.Model;
using WebApplicationDemoContext.DTO;

namespace WebApplicationDemoContext.Services.IServices;

public interface IServiceUser
{
    Task<Result<User>> AddUser(UserDTO request);
    Task<Result<UserLoginResponse>> Login(RequestUserLogin request);
    Task<Result<User>> GetUserByUserId(int userId);
    Task<Result<UserResponse>> UpdateUserByUserId(UpdateUserRequest request);
}