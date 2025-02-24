using WebApplicationDemoContext.Common;
using WebApplicationDemoContext.DTO;
using WebApplicationDemoContext.Repositories;
using WebApplicationDemoContext.Services.IServices;

namespace WebApplicationDemoContext.Services;

public class ServiceUser : IServiceUser
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public ServiceUser(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<Result<UserResponse>> AddUser(RequestUserCreate request)
    {
        if (await _userRepository.ExistsByEmail(request.Email))
            return Result<UserResponse>.Fail("Email already exists");

        var newUser = await _userRepository.CreateUser(request.Username, request.Email, request.Password);
        var userResponse = new UserResponse
        {
            Id = newUser.Id,
            Username = newUser.Username,
            Email = newUser.Email
        };

        return Result<UserResponse>.Ok(userResponse, "User created successfully");
    }
   
    public Task<Result<UserLoginResponse>> Login(RequestUserLogin request)
    {
        throw new NotImplementedException();
    }
}