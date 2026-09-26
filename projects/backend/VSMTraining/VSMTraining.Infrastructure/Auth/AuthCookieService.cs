using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace VSMTraining.Infrastructure.Auth;

public static class AuthCookieNames
{
    public const string AccessToken = "auth_token";
}

public class AuthCookieService
{
    private readonly IHostEnvironment _environment;

    public AuthCookieService(IHostEnvironment environment)
    {
        _environment = environment;
    }

    public void WriteAuthCookie(HttpResponse response, string token, DateTimeOffset expiresAt, bool rememberMe)
    {
        var options = BuildBaseOptions();
        if (rememberMe)
        {
            options.Expires = expiresAt;
        }

        response.Cookies.Append(AuthCookieNames.AccessToken, token, options);
    }

    public void ClearAuthCookie(HttpResponse response)
    {
        response.Cookies.Delete(AuthCookieNames.AccessToken, BuildBaseOptions());
    }

    private CookieOptions BuildBaseOptions() => new()
    {
        HttpOnly = true,
        Secure = !_environment.IsDevelopment(),
        SameSite = SameSiteMode.Lax,
        Path = "/"
    };
}