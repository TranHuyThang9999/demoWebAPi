using System.Text.Json.Serialization;
using ServiceStack.DataAnnotations;

namespace WebApplicationDemoContext.DTO
{
    public class RequestUserCreate
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class RequestUserLogin
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class UserLoginResponse
    {
        public string Token { get; set; }
    }

    public class UpdateUserRequest
    {
        [JsonIgnore]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        public string Email { get; set; }
    }
}