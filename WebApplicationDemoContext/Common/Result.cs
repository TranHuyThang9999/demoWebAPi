using System.Text.Json.Serialization;

namespace WebApplicationDemoContext.Common
{
    public class Result<T>
    {
        public Result(){}
        public Result(bool success, string message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public bool Success { get; set; }
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        public static Result<T> Ok(T data, string message = "Success") =>
            new Result<T> { Success = true, Data = data, Message = message };

        public static Result<T> Fail(string message) =>
            new Result<T> { Success = false, Message = message };
    }
}