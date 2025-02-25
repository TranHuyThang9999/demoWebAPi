using WebApplicationDemoContext.core.Model;

namespace WebApplicationDemoContext.Services.IServices;

public interface IServiceJWT
{
    string GenerateJwtToken(User user, Dictionary<string, string>? customClaims = null);
    Dictionary <string,string> GetUserIdFromToken(string token);
}
