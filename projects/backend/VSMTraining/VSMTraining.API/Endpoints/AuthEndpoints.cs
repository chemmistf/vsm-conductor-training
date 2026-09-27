using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VSMTraining.Application.Attempts;
using VSMTraining.Application.Auth;
using VSMTraining.Infrastructure.Auth;

namespace VSMTraining.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        group.MapPost("/register",
                async (RegisterRequest request, AuthService auth, AuthCookieService cookies, HttpResponse response) =>
                {
                    try
                    {
                        var (user, token, expiresAt) = await auth.RegisterAsync(request);
                        cookies.WriteAuthCookie(response, token, expiresAt, rememberMe: false);
                        return Results.Json(new AuthResponse(user, expiresAt),
                            statusCode: StatusCodes.Status201Created);
                    }
                    catch (AuthException ex)
                    {
                        return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                    }
                })
            .WithName("Register");

        group.MapPost("/login",
                async (LoginRequest request, AuthService auth, AuthCookieService cookies, HttpResponse response) =>
                {
                    try
                    {
                        var (user, token, expiresAt) = await auth.LoginAsync(request);
                        cookies.WriteAuthCookie(response, token, expiresAt, request.RememberMe);
                        return Results.Ok(new AuthResponse(user, expiresAt));
                    }
                    catch (AuthException ex)
                    {
                        return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                    }
                })
            .WithName("Login");

        group.MapPost("/logout", (AuthCookieService cookies, HttpResponse response) =>
            {
                cookies.ClearAuthCookie(response);
                return Results.NoContent();
            })
            .WithName("Logout");

        group.MapGet("/me", async (ClaimsPrincipal principal, AuthService auth) =>
            {
                var userId = GetUserId(principal);
                var user = await auth.GetCurrentUserAsync(userId);
                return user is null ? Results.Unauthorized() : Results.Ok(user);
            })
            .RequireAuthorization()
            .WithName("GetCurrentUser");

        group.MapPost("/password/forgot",
                async (ForgotPasswordRequest request, AuthService auth, IHostEnvironment env) =>
                {
                    string? resetUrl;
                    try
                    {
                        resetUrl = await auth.RequestPasswordResetAsync(request.Email);
                    }
                    catch (AuthException ex)
                    {
                        return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                    }

                    const string message = "Если аккаунт существует, мы отправили ссылку для восстановления пароля.";
                    if (env.IsDevelopment())
                    {
                        return Results.Json(new ForgotPasswordResponse(message, resetUrl),
                            statusCode: StatusCodes.Status202Accepted);
                    }

                    return Results.Json(new { message }, statusCode: StatusCodes.Status202Accepted);
                })
            .WithName("ForgotPassword");

        group.MapPost("/password/reset", async (ResetPasswordRequest request, AuthService auth) =>
            {
                try
                {
                    await auth.ResetPasswordAsync(request);
                    return Results.Ok();
                }
                catch (AuthException ex)
                {
                    return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                }
            })
            .WithName("ResetPassword");
    }

    private static Guid GetUserId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return value is not null && Guid.TryParse(value, out var id) ? id : Guid.Empty;
    }
}
