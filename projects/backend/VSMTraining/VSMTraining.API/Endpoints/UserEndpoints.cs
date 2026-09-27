using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VSMTraining.Application.Attempts;
using VSMTraining.Infrastructure.Runtime;

namespace VSMTraining.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
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
            .WithName("GetMyProfile");

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
            .WithName("GetUserCompetencies");
    }

    private static Guid? GetUserId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
