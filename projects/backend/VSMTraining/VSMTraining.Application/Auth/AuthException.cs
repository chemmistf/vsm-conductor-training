namespace VSMTraining.Application.Auth;

public class AuthException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public AuthException(string code, int statusCode, string message) : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}