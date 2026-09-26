using VSMTraining.Application.Attempts;
using VSMTraining.Infrastructure.Runtime;

namespace VSMTraining.API.Endpoints;

public static class AttemptEndpoints
{
    public static void MapAttemptEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/attempts").WithTags("Attempts");

        group.MapPost("/", async (StartAttemptRequest? request, AttemptFlowService flow) =>
            {
                try
                {
                    var result = await flow.StartAttemptAsync(request?.ScenarioId);
                    return Results.Ok(result);
                }
                catch (AttemptFlowException ex)
                {
                    return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                }
            })
            .WithName("StartAttempt");

        group.MapPost("/{attemptId:guid}/choice", async (Guid attemptId, ChooseRequest request, AttemptFlowService flow) =>
            {
                try
                {
                    var result = await flow.ChooseAsync(attemptId, request.ChoiceId);
                    return Results.Ok(result);
                }
                catch (AttemptFlowException ex)
                {
                    return Results.Json(new ApiErrorResponse(ex.Code, ex.Message), statusCode: ex.StatusCode);
                }
            })
            .WithName("ChooseAttemptOption");
    }
}