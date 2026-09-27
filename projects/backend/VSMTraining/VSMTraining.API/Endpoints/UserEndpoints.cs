using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VSMTraining.Application.Attempts;
using VSMTraining.Application.Competencies;
using VSMTraining.Application.Users;
using VSMTraining.Infrastructure.Runtime;

namespace VSMTraining.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Пользователи")
            .RequireAuthorization();

        group.MapGet("/me/profile", async (ClaimsPrincipal principal, AttemptFlowService flow) =>
            {
                var userId = GetUserId(principal);
                if (userId is null) return Results.Unauthorized();

                try
                {
                    return Results.Ok(await flow.GetProfileAsync(userId.Value));
                }
                catch (AttemptFlowException ex)
                {
                    return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                }
            })
            .WithName("GetMyProfile")
            .WithSummary("Получить профиль текущего пользователя")
            .WithDescription("Возвращает профиль сотрудника из текущей сессии: имя, email, класс обслуживания, " +
                             "уровень, общий и текущий XP, прогресс до следующего уровня, статус сертификации, " +
                             "количество завершённых попыток и последние достижения. Идентификатор пользователя в URL не требуется.")
            .Produces<ProfileResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/{userId:guid}/competencies", async (Guid userId, ClaimsPrincipal principal, AttemptFlowService flow) =>
            {
                var currentUserId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (!Guid.TryParse(currentUserId, out var authenticatedUserId)) return Results.Unauthorized();
                if (authenticatedUserId != userId) return Results.Forbid();

                try
                {
                    return Results.Ok(await flow.GetUserCompetenciesAsync(userId));
                }
                catch (AttemptFlowException ex)
                {
                    return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                }
            })
            .WithName("GetUserCompetencies")
            .WithSummary("Получить компетенции пользователя")
            .WithDescription("Возвращает накопленные компетенции пользователя и уровень каждой компетенции. " +
                             "Доступ разрешён только самому пользователю: переданный userId должен совпадать " +
                             "с идентификатором в текущей авторизованной сессии.")
            .Produces<UserCompetenciesResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }

    private static Guid? GetUserId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
