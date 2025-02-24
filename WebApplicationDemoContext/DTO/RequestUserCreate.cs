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
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserResponse
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }

    public class UserLoginResponse
    {
        public string Token { get; set; }
        public UserResponse User { get; set; }
    }
}