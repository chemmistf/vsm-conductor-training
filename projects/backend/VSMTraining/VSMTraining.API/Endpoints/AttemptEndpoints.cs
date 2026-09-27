using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VSMTraining.Application.Attempts;
using VSMTraining.Infrastructure.Runtime;

namespace VSMTraining.API.Endpoints;

public static class AttemptEndpoints
{
    public static void MapAttemptEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attempts")
            .WithTags("Attempts")
            .RequireAuthorization();

        group.MapPost("/", async (StartAttemptRequest? request, ClaimsPrincipal principal, AttemptFlowService flow) =>
            {
                if (!TryGetUserId(principal, out var userId)) return Results.Unauthorized();

                try
                {
                    var result = await flow.StartAttemptAsync(request?.ScenarioId, userId);
                    return Results.Ok(result);
                }
                catch (AttemptFlowException ex)
                {
                    return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                }
            })
            .WithName("StartAttempt");

        group.MapPost("/{attemptId:guid}/choice",
                async (Guid attemptId, ChooseRequest request, ClaimsPrincipal principal, AttemptFlowService flow) =>
                {
                    if (!TryGetUserId(principal, out var userId)) return Results.Unauthorized();

                    try
                    {
                        var result = await flow.ChooseAsync(attemptId, request.ChoiceId, userId);
                        return Results.Ok(result);
                    }
                    catch (AttemptFlowException ex)
                    {
                        return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                    }
                })
            .WithName("ChooseAttemptOption");

        group.MapPost("/{attemptId:guid}/timeout", async (Guid attemptId, ClaimsPrincipal principal, AttemptFlowService flow) =>
            {
                if (!TryGetUserId(principal, out var userId)) return Results.Unauthorized();

                try
                {
                    var result = await flow.TimeoutAsync(attemptId, userId);
                    return Results.Ok(result);
                }
                catch (AttemptFlowException ex)
                {
                    return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                }
            })
            .WithName("TimeoutAttempt");

        group.MapGet("/{attemptId:guid}/result", async (Guid attemptId, ClaimsPrincipal principal, AttemptFlowService flow) =>
            {
                if (!TryGetUserId(principal, out var userId)) return Results.Unauthorized();

                try
                {
                    var result = await flow.GetResultAsync(attemptId, userId);
                    return Results.Ok(result);
                }
                catch (AttemptFlowException ex)
                {
                    return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                }
            })
            .WithName("GetAttemptResult");
    }

    private static bool TryGetUserId(ClaimsPrincipal principal, out Guid userId)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(value, out userId);
    }
}
