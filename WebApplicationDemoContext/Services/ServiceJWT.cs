using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApplicationDemoContext.core.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApplicationDemoContext.Services.IServices;

public class ServiceJWT : IServiceJWT
{
     private readonly IConfiguration _configuration;
    private readonly ILogger<ServiceJWT> _logger;

    public ServiceJWT(IConfiguration configuration, ILogger<ServiceJWT> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateJwtToken(User user, Dictionary<string, string>? customClaims = null)
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
                new Claim(ClaimTypes.Name, user.Name)
            };

            // Thêm các claim tùy chỉnh nếu có
            if (customClaims != null)
            {
                foreach (var customClaim in customClaims)
                {
                    claims.Add(new Claim(customClaim.Key, customClaim.Value));
                }
            }

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