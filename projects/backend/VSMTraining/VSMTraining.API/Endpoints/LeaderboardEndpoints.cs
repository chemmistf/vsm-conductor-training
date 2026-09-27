using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VSMTraining.Application.Attempts;
using VSMTraining.Application.Leaderboard;
using VSMTraining.Infrastructure.Runtime;

namespace VSMTraining.API.Endpoints;

public static class LeaderboardEndpoints
{
    public static void MapLeaderboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/leaderboard")
            .WithTags("Рейтинг")
            .RequireAuthorization();

        group.MapGet("/", async (
                string? period,
                string? scope,
                ClaimsPrincipal principal,
                AttemptFlowService flow) =>
            {
                var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
                if (!Guid.TryParse(userId, out var authenticatedUserId)) return Results.Unauthorized();

                try
                {
                    return Results.Ok(await flow.GetLeaderboardAsync(authenticatedUserId, period, scope));
                }
                catch (AttemptFlowException ex)
                {
                    return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                }
            })
            .WithName("GetLeaderboard")
            .WithSummary("Получить таблицу лидеров")
            .WithDescription("Возвращает до 20 лучших пользователей и отдельно добавляет текущего пользователя, если он находится ниже топ-20. " +
                             "Параметр period принимает week или all_time: для week считается XP за последние 7 дней, " +
                             "для all_time — накопленный XP. Параметр scope ограничивает выборку: brigade, depot, company или friends. " +
                             "Если параметры не распознаны, используются значения по умолчанию all_time и brigade.")
            .Produces<LeaderboardResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized);
    }
}
