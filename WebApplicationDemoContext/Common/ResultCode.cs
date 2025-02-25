namespace WebApplicationDemoContext.Common;

public enum ResultCode
{
    Success = 0,
    ErrorDb = 1,
    ErrorValidation = 2,
    ErrorUnauthorized = 3,
    ErrorNotFound = 4,
    ErrorServer = 5,
    ErrorConflict = 6
}