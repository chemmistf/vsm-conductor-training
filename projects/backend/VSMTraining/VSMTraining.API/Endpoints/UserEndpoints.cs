using VSMTraining.Application.Attempts;
using VSMTraining.Infrastructure.Runtime;

namespace VSMTraining.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/{userId:guid}/competencies", async (Guid userId, AttemptFlowService flow) =>
            {
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
}
