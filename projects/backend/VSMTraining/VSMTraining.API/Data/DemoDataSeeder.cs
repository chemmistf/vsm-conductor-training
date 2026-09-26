using Microsoft.EntityFrameworkCore;
using VSMTraining.Application.Attempts;
using VSMTraining.Application.Scenarios;
using VSMTraining.Domain.Enums;
using VSMTraining.Domain.Scenarios;
using VSMTraining.Domain.Users;
using VSMTraining.Infrastructure.Persistence;

namespace VSMTraining.API.Data;

public static class DemoDataSeeder
{
    private const string ScenarioTitle = "Нетрезвый пассажир";
    private const string ContentFileName = "intoxicated_passenger_v1.json";

    public static async Task SeedAsync(AppDbContext db)
    {
        var contentPath = Path.Combine(AppContext.BaseDirectory, "Data", ContentFileName);
        var contentJson = await File.ReadAllTextAsync(contentPath);

        var content = ScenarioRuntime.Parse(contentJson);
        var errors = ScenarioRuntime.Validate(content);
        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                $"Scenario file '{ContentFileName}' failed validation: {string.Join(" ", errors)}");
        }

        var now = DateTimeOffset.UtcNow;

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == DemoDataIds.DemoUserId);
        if (user is null)
        {
            user = new User
            {
                Id = DemoDataIds.DemoUserId,
                ExternalId = "demo-user",
                Name = "Demo User",
                Level = 1,
                Xp = 0,
                CertificationStatus = CertificationStatus.None,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Users.Add(user);
        }

        var scenario = await db.Scenarios.FirstOrDefaultAsync(s => s.Code == DemoDataIds.ScenarioCode);
        if (scenario is null)
        {
            scenario = new Scenario
            {
                Id = Guid.NewGuid(),
                Code = DemoDataIds.ScenarioCode,
                Title = ScenarioTitle,
                Description = "Демонстрационный сценарий для хакатона.",
                Difficulty = ScenarioDifficulty.Medium,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Scenarios.Add(scenario);
        }

        var version = await db.ScenarioVersions
            .FirstOrDefaultAsync(v => v.ScenarioId == scenario.Id && v.Version == DemoDataIds.ScenarioVersion);

        if (version is null)
        {
            db.ScenarioVersions.Add(new ScenarioVersion
            {
                Id = Guid.NewGuid(),
                ScenarioId = scenario.Id,
                Version = DemoDataIds.ScenarioVersion,
                ContentJson = contentJson,
                CreatedAt = now,
                IsPublished = true
            });
        }
        else
        {
            // Позволяет поправить текст/дельты в JSON ночью и перезапустить backend без ручных SQL.
            version.ContentJson = contentJson;
            version.IsPublished = true;
            version.UpdatedAt = now;
        }

        await db.SaveChangesAsync();
    }
}