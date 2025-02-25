using System.Globalization;
using WebApplicationDemoContext.Common;
using WebApplicationDemoContext.core.Model;
using WebApplicationDemoContext.DTO;
using WebApplicationDemoContext.Services.IServices;
using WebApplicationDemoContext.Core.Repositories;

namespace WebApplicationDemoContext.Services;

public class ServiceUser : IServiceUser
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ServiceUser> _logger;
    private readonly IServiceJWT _serviceJWT;

    public ServiceUser(IUserRepository userRepository, ILogger<ServiceUser> logger, IServiceJWT serviceJwt)
    {
        _userRepository = userRepository;
        _logger = logger;
        _serviceJWT = serviceJwt;
    }

    public async Task<Result<User>> AddUser(RequestUserCreate request)
    {
        var user = await _userRepository.GetUserByName(request.Username);
        if (user != null)
        {
            return Result<User>.Fail("name already exists");
        }

        var userResponse = new User
        {
            Id = Random.Shared.Next(1, 101),
            Name = request.Username,
            Email = request.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Created = DateOnly.FromDateTime(DateTime.UtcNow),
            Updated = DateTime.UtcNow
        };

        var newUser = await _userRepository.CreateUser(userResponse);

        return Result<User>.Ok(newUser, "User created successfully");
    }

    public async Task<Result<UserLoginResponse>> Login(RequestUserLogin request)
    {
        var user = await _userRepository.GetUserByName(request.Name);
        if (user == null)
        {
            return Result<UserLoginResponse>.Fail($"username or password not match");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Result<UserLoginResponse>.Fail("username or password not match 2");
        }

        var customClaims = new Dictionary<string, string>
        {
            { "userID", user.Id.ToString() },
            { "lastPasswordUpdate", user.Updated.ToString(CultureInfo.InvariantCulture) }
        };

        var token = _serviceJWT.GenerateJwtToken(user, customClaims);
        var response = new UserLoginResponse
        {
            Token = token,
        };

        return Result<UserLoginResponse>.Ok(response, "Login successful");
    }

    public async Task<Result<User>> GetUserByUserId(int userID)
    {
        var user = await _userRepository.GetUserByID(userID);

        return user == null ? Result<User>.Fail("User not found") : Result<User>.Ok(user);
    }

    public async Task<Result<UserResponse>> UpdateUserByUserId(UpdateUserRequest request)
    {
        try
        {
            var userResponse = new User
            {
                Id = request.Id,
                Name = request.Username,
                Email = request.Email,
                Updated = DateTime.UtcNow
            };
            _logger.LogInformation($"Updating user  : {request.Id} {request.Username} {request.Email}");
            var status = await _userRepository.UpdateUserByID(userResponse);
            if (!status)
            {
                return Result<UserResponse>.Fail("User not found");
            }

            return Result<UserResponse>.Ok(null, "User updated successfully");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    public async Task<Result<UserResponse>> DeleteUserByUserID(int userID)
    {
        return new Result<UserResponse>();
    }
}