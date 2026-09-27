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
        var group = app.MapGroup("/api/auth").WithTags("Авторизация");

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
            .WithName("Register")
            .WithSummary("Зарегистрировать пользователя")
            .WithDescription("Создаёт новую учётную запись сотрудника, проверяет имя, email и пароль, " +
                             "после чего сразу устанавливает HttpOnly-cookie с токеном авторизации. " +
                             "Повторная регистрация на уже занятый email невозможна.")
            .Produces<AuthResponse>(StatusCodes.Status201Created)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity);

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
            .WithName("Login")
            .WithSummary("Войти в аккаунт")
            .WithDescription("Проверяет email и пароль пользователя и устанавливает HttpOnly-cookie с JWT-токеном. " +
                             "Параметр rememberMe определяет срок действия авторизации. " +
                             "Cookie автоматически используется защищёнными эндпоинтами API.")
            .Produces<AuthResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status401Unauthorized)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity);

        group.MapPost("/logout", (AuthCookieService cookies, HttpResponse response) =>
            {
                cookies.ClearAuthCookie(response);
                return Results.NoContent();
            })
            .WithName("Logout")
            .WithSummary("Выйти из аккаунта")
            .WithDescription("Удаляет cookie авторизации в браузере. Операция идемпотентна: если cookie уже отсутствует, " +
                             "сервер всё равно возвращает успешный ответ.")
            .Produces(StatusCodes.Status204NoContent);

        group.MapGet("/me", async (ClaimsPrincipal principal, AuthService auth) =>
            {
                var userId = GetUserId(principal);
                var user = await auth.GetCurrentUserAsync(userId);
                return user is null ? Results.Unauthorized() : Results.Ok(user);
            })
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithSummary("Получить текущего пользователя")
            .WithDescription("Возвращает минимальные данные пользователя, которому принадлежит текущая сессия. " +
                             "Идентификатор берётся из JWT в cookie, поэтому передавать его в запросе не нужно.")
            .Produces<UserDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

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
            .WithName("ForgotPassword")
            .WithSummary("Запросить восстановление пароля")
            .WithDescription("Проверяет email и отправляет пользователю ссылку для восстановления пароля. " +
                             "В целях безопасности ответ одинаков для существующего и несуществующего аккаунта. " +
                             "Ссылка действует 20 минут. В режиме разработки ответ дополнительно содержит DebugResetUrl, " +
                             "в production это поле не возвращается.")
            .Produces<ForgotPasswordResponse>(StatusCodes.Status202Accepted)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity);

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
            .WithName("ResetPassword")
            .WithSummary("Установить новый пароль")
            .WithDescription("Проверяет одноразовый токен из ссылки восстановления и меняет пароль пользователя. " +
                             "Токен нельзя использовать повторно; пароль и его подтверждение должны совпадать и содержать " +
                             "не менее 8 символов.")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity);
    }

    private static Guid GetUserId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return value is not null && Guid.TryParse(value, out var id) ? id : Guid.Empty;
    }
}
