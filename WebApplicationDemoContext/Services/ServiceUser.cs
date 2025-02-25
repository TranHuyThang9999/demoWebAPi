using WebApplicationDemoContext.Common;
using WebApplicationDemoContext.core.Model;
using WebApplicationDemoContext.DTO;
using WebApplicationDemoContext.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplicationDemoContext.Core.Repositories;

namespace WebApplicationDemoContext.Services;

public class ServiceUser : IServiceUser
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ServiceUser> _logger;
    private readonly IConfiguration _configuration;

    public ServiceUser(IUserRepository userRepository, IConfiguration configuration, ILogger<ServiceUser> logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
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

        var token = GenerateJwtToken(user);
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

    private string GenerateJwtToken(User user)
    {
        try
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null when generating JWT.");
            }

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Birthdate, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var secretKey = _configuration["Jwt:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is missing in configuration.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirationConfig = _configuration["Jwt:Expiration"];
            if (string.IsNullOrEmpty(expirationConfig) || !double.TryParse(expirationConfig, out double expiration))
            {
                throw new InvalidOperationException("Invalid or missing JWT Expiration value.");
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiration),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error occurred while generating JWT token: {Message}", ex.Message);
            throw;
        }
    }
}