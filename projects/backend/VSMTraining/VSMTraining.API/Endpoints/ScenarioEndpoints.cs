using Microsoft.EntityFrameworkCore;
using VSMTraining.Application.Scenarios;
using VSMTraining.Infrastructure.Persistence;

namespace VSMTraining.API.Endpoints;

public static class ScenarioEndpoints
{
    public static void MapScenarioEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/scenarios")
            .WithTags("Scenarios")
            .RequireAuthorization();

        group.MapGet("", async (AppDbContext db) =>
        {
            var scenarios = await db.Scenarios
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Title)
                .Select(s => new ScenarioSummaryDto(
                    s.Id,
                    s.Code,
                    s.Title,
                    s.Description,
                    s.Category,
                    s.Difficulty.ToString()))
                .ToListAsync();

            return Results.Ok(scenarios);
        }).WithName("GetScenarios");
    }
}

