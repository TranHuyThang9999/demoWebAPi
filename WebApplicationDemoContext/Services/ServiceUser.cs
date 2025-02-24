using WebApplicationDemoContext.Common;
using WebApplicationDemoContext.core.Model;
using WebApplicationDemoContext.DTO;
using WebApplicationDemoContext.Repositories;
using WebApplicationDemoContext.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApplicationDemoContext.Services;

public class ServiceUser : IServiceUser
{
    private readonly IUserRepository _userRepository;

    public ServiceUser(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    private readonly IConfiguration _configuration;

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

    public async Task<Result<User>> GetUserByUserID(int userID)
    {
        var user = await _userRepository.GetUserByID(userID);

        return user == null ? Result<User>.Fail("User not found") : Result<User>.Ok(user);
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}