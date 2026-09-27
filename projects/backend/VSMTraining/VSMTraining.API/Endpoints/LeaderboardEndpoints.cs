using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VSMTraining.Application.Attempts;
using VSMTraining.Infrastructure.Runtime;

namespace VSMTraining.API.Endpoints;

public static class LeaderboardEndpoints
{
    public static void MapLeaderboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/leaderboard")
            .WithTags("Leaderboard")
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
            .WithName("GetLeaderboard");
    }
}
