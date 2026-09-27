using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VSMTraining.Application.Attempts;
using VSMTraining.Application.Competencies;
using VSMTraining.Application.Scenarios;
using VSMTraining.Application.Users;
using VSMTraining.Domain.Attempts;
using VSMTraining.Domain.Competencies;
using VSMTraining.Domain.Enums;
using VSMTraining.Infrastructure.Persistence;

namespace VSMTraining.Infrastructure.Runtime;

public class AttemptFlowService
{
    private readonly AppDbContext _db;

    public AttemptFlowService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AttemptStateResponse> StartAttemptAsync(Guid? scenarioId, Guid userId)
    {
        var scenario = scenarioId.HasValue
            ? await _db.Scenarios.FirstOrDefaultAsync(s => s.Id == scenarioId.Value)
            : await _db.Scenarios.FirstOrDefaultAsync(s => s.Code == DemoDataIds.ScenarioCode);

        if (scenario is null)
        {
            throw new AttemptFlowException("scenario_not_found", StatusCodes.NotFound, "Scenario does not exist.");
        }

        var version = await _db.ScenarioVersions
            .Where(v => v.ScenarioId == scenario.Id && v.IsPublished)
            .OrderByDescending(v => v.Version)
            .FirstOrDefaultAsync();

        if (version is null)
        {
            throw new AttemptFlowException("scenario_not_published", StatusCodes.Conflict,
                "Scenario has no published version.");
        }

        var content = ScenarioRuntime.Parse(version.ContentJson);
        var errors = ScenarioRuntime.Validate(content);
        if (errors.Count > 0)
        {
            throw new AttemptFlowException("scenario_invalid", StatusCodes.InternalServerError,
                string.Join(" ", errors));
        }

        var startNode = ScenarioRuntime.GetNode(content, content.StartNode);
        var now = DateTimeOffset.UtcNow;

        var attempt = new Attempt
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ScenarioVersionId = version.Id,
            Mode = AttemptMode.Training,
            StartedAt = now,
            CurrentNodeId = content.StartNode,
            LifecycleStatus = AttemptLifecycleStatus.InProgress,
            ResultStatus = null,
            InitialSafety = content.InitialState.Safety,
            CurrentSafety = content.InitialState.Safety,
            InitialLoyalty = content.InitialState.Loyalty,
            CurrentLoyalty = content.InitialState.Loyalty,
            CurrentNodeStartedAt = now,
            NodeDeadlineAt = startNode.TimerSeconds.HasValue ? now.AddSeconds(startNode.TimerSeconds.Value) : null,
            CreatedAt = now,
            UpdatedAt = now
        };

        _db.Attempts.Add(attempt);
        await _db.SaveChangesAsync();

        return BuildStateResponse(attempt, startNode);
    }

    public async Task<AttemptStateResponse> ChooseAsync(Guid attemptId, string choiceId, Guid userId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        var attempt = await _db.Attempts
            .FromSqlInterpolated($"SELECT * FROM attempts WHERE \"Id\" = {attemptId} AND \"UserId\" = {userId} FOR UPDATE")
            .Include(a => a.ScenarioVersion)
            .FirstOrDefaultAsync();

        if (attempt is null)
        {
            throw new AttemptFlowException("attempt_not_found", StatusCodes.NotFound, "Attempt does not exist.");
        }

        if (attempt.LifecycleStatus != AttemptLifecycleStatus.InProgress)
        {
            throw new AttemptFlowException("attempt_finished", StatusCodes.Conflict, "Attempt is already finished.");
        }

        var content = ScenarioRuntime.Parse(attempt.ScenarioVersion.ContentJson);
        var currentNode = ScenarioRuntime.GetNode(content, attempt.CurrentNodeId!);
        var now = DateTimeOffset.UtcNow;

        if (attempt.NodeDeadlineAt.HasValue && now >= attempt.NodeDeadlineAt.Value)
        {
            var timedOutNode = await ApplyTimeoutTransitionAsync(attempt, content, currentNode, now);
            await _db.SaveChangesAsync();
            return BuildStateResponse(attempt, timedOutNode);
        }

        var choice = currentNode.Choices?.FirstOrDefault(c => c.Id == choiceId);
        if (choice is null)
        {
            throw new AttemptFlowException("choice_not_found", StatusCodes.UnprocessableEntity,
                "Choice does not exist in the current node.");
        }

        var safetyBefore = attempt.CurrentSafety;
        var loyaltyBefore = attempt.CurrentLoyalty;
        var resolution = ScenarioRuntime.ResolveChoice(choice, safetyBefore, loyaltyBefore);
        var responseTimeMs = (int)(now - (attempt.CurrentNodeStartedAt ?? attempt.StartedAt)).TotalMilliseconds;

        _db.AttemptEvents.Add(new AttemptEvent
        {
            AttemptId = attempt.Id,
            NodeId = attempt.CurrentNodeId!,
            ChoiceId = choice.Id,
            EventType = AttemptEventType.UserChoice,
            OccurredAt = now,
            ResponseTimeMs = responseTimeMs,
            SafetyBefore = safetyBefore,
            SafetyDelta = choice.SafetyDelta,
            SafetyAfter = resolution.SafetyAfter,
            LoyaltyBefore = loyaltyBefore,
            LoyaltyDelta = choice.LoyaltyDelta,
            LoyaltyAfter = resolution.LoyaltyAfter,
            CriticalError = choice.CriticalError,
            CriticalErrorCode = choice.CriticalErrorCode,
            EventDataJson = JsonSerializer.Serialize(new { competencies = choice.Competencies })
        });

        attempt.CurrentSafety = resolution.SafetyAfter;
        attempt.CurrentLoyalty = resolution.LoyaltyAfter;

        var nextNodeId = choice.CriticalError ? "critical_failure" : resolution.NextNodeId;
        attempt.CurrentNodeId = nextNodeId;
        var nextNode = ScenarioRuntime.GetNode(content, nextNodeId);

        if (nextNode.Type == ScenarioNodeTypes.Result)
        {
            await FinishAttemptAsync(attempt, nextNode, now, choice.Competencies);
        }
        else
        {
            attempt.CurrentNodeStartedAt = now;
            attempt.NodeDeadlineAt =
                nextNode.TimerSeconds.HasValue ? now.AddSeconds(nextNode.TimerSeconds.Value) : null;
        }

        attempt.UpdatedAt = now;
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return BuildStateResponse(attempt, nextNode);
    }

    public async Task<ResultResponse> GetResultAsync(Guid attemptId, Guid userId)
    {
        var attempt = await _db.Attempts
            .Include(a => a.ScenarioVersion)
            .Include(a => a.User)
            .Include(a => a.Events.OrderBy(e => e.OccurredAt))
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId);

        if (attempt is null)
        {
            throw new AttemptFlowException("attempt_not_found", StatusCodes.NotFound, "Attempt does not exist.");
        }

        if (attempt.LifecycleStatus != AttemptLifecycleStatus.Finished)
        {
            throw new AttemptFlowException("attempt_not_finished", StatusCodes.Conflict,
                "Attempt has not finished yet.");
        }

        var content = ScenarioRuntime.Parse(attempt.ScenarioVersion.ContentJson);
        var resultNode = ScenarioRuntime.GetNode(content, attempt.CurrentNodeId!);

        var gameplayEvents = attempt.Events
            .Where(e => e.EventType != AttemptEventType.AttemptFinished)
            .ToList();

        var criticalErrors = gameplayEvents
            .Where(e => e.CriticalError && e.ChoiceId is not null)
            .Select(e =>
            {
                var node = ScenarioRuntime.GetNode(content, e.NodeId);
                var choice = node.Choices?.First(c => c.Id == e.ChoiceId)!;
                return new CriticalErrorDto(e.NodeId, e.ChoiceId!, e.CriticalErrorCode, choice.Text);
            })
            .ToList();

        var competencyTotals = ScenarioRuntime.AggregateCompetencies(gameplayEvents.Select(e => e.EventDataJson));
        var competencies = competencyTotals
            .Select(pair => new CompetencyResultDto(
                pair.Key,
                CompetencyCatalog.GetName(pair.Key),
                pair.Value,
                ScenarioRuntime.CompetencyLevel(pair.Value)))
            .ToList();

        var xp = XpCalculator.Calculate(
            ToResultStatusString(attempt.ResultStatus)!,
            competencyTotals.Values);

        var timeline = gameplayEvents
            .Select(e => BuildTimelineEntry(content, e))
            .ToList();

        return new ResultResponse(
            attempt.Id,
            ToResultStatusString(attempt.ResultStatus)!,
            resultNode.Text,
            xp.Total,
            attempt.User!.Xp,
            attempt.User.Level,
            new ScaleSummaryDto(attempt.InitialSafety, attempt.FinalSafety ?? attempt.CurrentSafety),
            new ScaleSummaryDto(attempt.InitialLoyalty, attempt.FinalLoyalty ?? attempt.CurrentLoyalty),
            criticalErrors,
            competencies,
            timeline);
    }

    private static TimelineEntryDto BuildTimelineEntry(ScenarioContent content, AttemptEvent entry)
    {
        var node = ScenarioRuntime.GetNode(content, entry.NodeId);
        var choice = entry.ChoiceId is null
            ? null
            : node.Choices?.FirstOrDefault(c => c.Id == entry.ChoiceId);

        var nextNodeId = entry.CriticalError
            ? "critical_failure"
            : choice is not null
                ? ScenarioRuntime.ResolveConditionalTransition(
                    choice.NextNode,
                    choice.ConditionalNext,
                    false,
                    entry.SafetyAfter ?? entry.SafetyBefore ?? 0,
                    entry.LoyaltyAfter ?? entry.LoyaltyBefore ?? 0)
                : node.TimeoutOutcome is not null
                    ? ScenarioRuntime.ResolveConditionalTransition(
                        node.TimeoutOutcome.NextNode,
                        node.TimeoutOutcome.ConditionalNext,
                        false,
                        entry.SafetyAfter ?? entry.SafetyBefore ?? 0,
                        entry.LoyaltyAfter ?? entry.LoyaltyBefore ?? 0)
                    : null;

        var outcomeText = nextNodeId is null ? null : ScenarioRuntime.GetNode(content, nextNodeId).Text;

        return new TimelineEntryDto(
            entry.NodeId,
            entry.ChoiceId,
            node.Text,
            choice?.Text,
            outcomeText,
            entry.SafetyBefore ?? 0,
            entry.SafetyDelta ?? 0,
            entry.SafetyAfter ?? entry.SafetyBefore ?? 0,
            entry.LoyaltyBefore ?? 0,
            entry.LoyaltyDelta ?? 0,
            entry.LoyaltyAfter ?? entry.LoyaltyBefore ?? 0,
            ScenarioRuntime.AggregateCompetencies(new[] {entry.EventDataJson}),
            entry.CriticalError,
            entry.CriticalErrorCode);
    }

    public async Task<UserCompetenciesResponse> GetUserCompetenciesAsync(Guid userId)
    {
        if (!await _db.Users.AnyAsync(u => u.Id == userId))
        {
            throw new AttemptFlowException("user_not_found", StatusCodes.NotFound, "User does not exist.");
        }

        var competencies = await _db.UserCompetencies
            .AsNoTracking()
            .Include(uc => uc.Competency)
            .Where(uc => uc.UserId == userId)
            .OrderBy(uc => uc.Competency.Code)
            .Select(uc => new UserCompetencyDto(
                uc.Competency.Code,
                uc.Competency.Name,
                uc.Score,
                ToUserCompetencyLevel(uc.Level)))
            .ToListAsync();

        return new UserCompetenciesResponse(userId, competencies);
    }

    public async Task<ProfileResponse> GetProfileAsync(Guid userId)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Include(u => u.Attempts)
                .ThenInclude(a => a.ScenarioVersion)
                    .ThenInclude(v => v.Scenario)
            .SingleOrDefaultAsync(u => u.Id == userId);

        if (user is null)
        {
            throw new AttemptFlowException("user_not_found", StatusCodes.NotFound, "User does not exist.");
        }

        var level = XpCalculator.CalculateLevel(user.Xp);
        var levelStartXp = (level - 1) * 500L;
        var xpInLevel = Math.Max(0, user.Xp - levelStartXp);
        var xpToNextLevel = Math.Max(0, 500 - xpInLevel);
        var levelProgressPercent = Math.Clamp((int)Math.Round(xpInLevel / 500d * 100), 0, 100);
        var completedAttempts = user.Attempts
            .Where(a => a.LifecycleStatus == AttemptLifecycleStatus.Finished && a.FinishedAt.HasValue)
            .OrderByDescending(a => a.FinishedAt)
            .ToList();

        var achievements = completedAttempts
            .Take(3)
            .Select(attempt => new ProfileAchievementDto(
                attempt.Id.ToString(),
                attempt.ScenarioVersion.Scenario.Title,
                "Сценарий завершён",
                attempt.FinishedAt))
            .ToList();

        return new ProfileResponse(
            user.Id,
            user.Name,
            user.Email,
            user.CurrentServiceClass,
            level,
            user.Xp,
            xpInLevel,
            xpToNextLevel,
            levelProgressPercent,
            user.CertificationStatus.ToString(),
            completedAttempts.Count,
            achievements);
    }

    private async Task<ScenarioNode> ApplyTimeoutTransitionAsync(Attempt attempt, ScenarioContent content,
        ScenarioNode currentNode,
        DateTimeOffset now)
    {
        var timeoutOutcome = currentNode.TimeoutOutcome!;
        var safetyBefore = attempt.CurrentSafety;
        var loyaltyBefore = attempt.CurrentLoyalty;
        var safetyAfter = Math.Clamp(safetyBefore + timeoutOutcome.SafetyDelta, 0, 100);
        var loyaltyAfter = Math.Clamp(loyaltyBefore + timeoutOutcome.LoyaltyDelta, 0, 100);

        _db.AttemptEvents.Add(new AttemptEvent
        {
            AttemptId = attempt.Id,
            NodeId = attempt.CurrentNodeId!,
            ChoiceId = null,
            EventType = AttemptEventType.Timeout,
            OccurredAt = now,
            ResponseTimeMs = (int)(now - (attempt.CurrentNodeStartedAt ?? attempt.StartedAt)).TotalMilliseconds,
            SafetyBefore = safetyBefore,
            SafetyDelta = timeoutOutcome.SafetyDelta,
            SafetyAfter = safetyAfter,
            LoyaltyBefore = loyaltyBefore,
            LoyaltyDelta = timeoutOutcome.LoyaltyDelta,
            LoyaltyAfter = loyaltyAfter,
            CriticalError = timeoutOutcome.CriticalError,
            CriticalErrorCode = timeoutOutcome.CriticalErrorCode,
            EventDataJson = JsonSerializer.Serialize(new
            {
                competencies = timeoutOutcome.Competencies
            })
        });

        attempt.CurrentSafety = safetyAfter;
        attempt.CurrentLoyalty = loyaltyAfter;

        var nextNodeId = timeoutOutcome.CriticalError
            ? "critical_failure"
            : ScenarioRuntime.ResolveConditionalTransition(
                timeoutOutcome.NextNode,
                timeoutOutcome.ConditionalNext,
                false,
                safetyAfter,
                loyaltyAfter);
        attempt.CurrentNodeId = nextNodeId;
        var nextNode = ScenarioRuntime.GetNode(content, nextNodeId);

        if (nextNode.Type == ScenarioNodeTypes.Result)
        {
            await FinishAttemptAsync(attempt, nextNode, now, timeoutOutcome.Competencies);
        }
        else
        {
            attempt.CurrentNodeStartedAt = now;
            attempt.NodeDeadlineAt =
                nextNode.TimerSeconds.HasValue ? now.AddSeconds(nextNode.TimerSeconds.Value) : null;
        }

        attempt.UpdatedAt = now;
        return nextNode;
    }

    public async Task<AttemptStateResponse> TimeoutAsync(Guid attemptId, Guid userId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        var attempt = await _db.Attempts
            .FromSqlInterpolated($"SELECT * FROM attempts WHERE \"Id\" = {attemptId} AND \"UserId\" = {userId} FOR UPDATE")
            .Include(a => a.ScenarioVersion)
            .FirstOrDefaultAsync();

        if (attempt is null)
            throw new AttemptFlowException("attempt_not_found", StatusCodes.NotFound, "Attempt does not exist");

        if (attempt.LifecycleStatus != AttemptLifecycleStatus.InProgress)
            throw new AttemptFlowException("attempt_finished", StatusCodes.Conflict, "Attempt is already finished.");

        var content = ScenarioRuntime.Parse(attempt.ScenarioVersion.ContentJson);
        var currentNode = ScenarioRuntime.GetNode(content, attempt.CurrentNodeId!);

        if (!currentNode.TimerSeconds.HasValue || currentNode.TimeoutOutcome is null)
        {
            throw new AttemptFlowException("node_has_no_timer", StatusCodes.Conflict,
                "Current node has no timer configured.");
        }

        var now = DateTimeOffset.UtcNow;
        if (!attempt.NodeDeadlineAt.HasValue || now < attempt.NodeDeadlineAt.Value)
        {
            throw new AttemptFlowException("timer_not_expired", StatusCodes.Conflict, "Timer has not expired yet.");
        }

        var nextNode = await ApplyTimeoutTransitionAsync(attempt, content, currentNode, now);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return BuildStateResponse(attempt, nextNode);
    }

    private async Task FinishAttemptAsync(
        Attempt attempt,
        ScenarioNode resultNode,
        DateTimeOffset now,
        IReadOnlyDictionary<string, int> currentCompetencies)
    {
        attempt.LifecycleStatus = AttemptLifecycleStatus.Finished;
        attempt.ResultStatus = ScenarioRuntime.ParseResultStatus(resultNode.ResultStatus!);
        attempt.FinishedAt = now;
        attempt.FinalSafety = attempt.CurrentSafety;
        attempt.FinalLoyalty = attempt.CurrentLoyalty;
        attempt.NodeDeadlineAt = null;
        attempt.CurrentNodeStartedAt = null;

        var competencyTotals = await AggregateAttemptCompetenciesAsync(attempt.Id, currentCompetencies);
        var competencyEntities = await EnsureCompetenciesAsync(competencyTotals.Keys, now);
        await PersistAttemptCompetenciesAsync(attempt, competencyTotals, competencyEntities, now);
        await ApplyUserCompetencySignalsAsync(attempt, competencyTotals, competencyEntities, now);

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == attempt.UserId);
        if (user is null)
        {
            throw new AttemptFlowException("user_not_found", StatusCodes.NotFound, "User does not exist.");
        }

        var xp = XpCalculator.Calculate(
            ToResultStatusString(attempt.ResultStatus)!,
            competencyTotals.Values);
        user.Xp += xp.Total;
        user.Level = XpCalculator.CalculateLevel(user.Xp);
        user.UpdatedAt = now;

        _db.AttemptEvents.Add(new AttemptEvent
        {
            AttemptId = attempt.Id,
            NodeId = attempt.CurrentNodeId!,
            EventType = AttemptEventType.AttemptFinished,
            OccurredAt = now,
            EventDataJson = JsonSerializer.Serialize(new
            {
                xp = new
                {
                    completion = xp.CompletionXp,
                    result = xp.ResultBonus,
                    competencies = xp.CompetencyBonus,
                    total = xp.Total
                }
            })
        });
    }

    private async Task<Dictionary<string, int>> AggregateAttemptCompetenciesAsync(
        Guid attemptId,
        IReadOnlyDictionary<string, int> currentCompetencies)
    {
        var eventData = await _db.AttemptEvents
            .AsNoTracking()
            .Where(e => e.AttemptId == attemptId)
            .Select(e => e.EventDataJson)
            .ToListAsync();

        eventData.Add(JsonSerializer.Serialize(new { competencies = currentCompetencies }));
        return ScenarioRuntime.AggregateCompetencies(eventData);
    }

    private async Task<Dictionary<string, Competency>> EnsureCompetenciesAsync(
        IEnumerable<string> codes,
        DateTimeOffset now)
    {
        var codeList = codes.ToList();
        var competencyEntities = await _db.Competencies
            .Where(c => codeList.Contains(c.Code))
            .ToDictionaryAsync(c => c.Code);

        foreach (var code in codeList)
        {
            if (competencyEntities.ContainsKey(code))
            {
                continue;
            }

            var competency = new Competency
            {
                Id = Guid.NewGuid(),
                Code = code,
                Name = CompetencyCatalog.GetName(code),
                IsActive = true,
                CreatedAt = now
            };
            _db.Competencies.Add(competency);
            competencyEntities[code] = competency;
        }

        return competencyEntities;
    }

    private async Task PersistAttemptCompetenciesAsync(
        Attempt attempt,
        IReadOnlyDictionary<string, int> competencyTotals,
        IReadOnlyDictionary<string, Competency> competencyEntities,
        DateTimeOffset now)
    {
        var existing = await _db.AttemptCompetencies
            .Where(ac => ac.AttemptId == attempt.Id)
            .ToDictionaryAsync(ac => ac.CompetencyId);

        foreach (var (code, score) in competencyTotals)
        {
            var competency = competencyEntities[code];
            if (!existing.TryGetValue(competency.Id, out var attemptCompetency))
            {
                attemptCompetency = new AttemptCompetency
                {
                    Id = Guid.NewGuid(),
                    AttemptId = attempt.Id,
                    CompetencyId = competency.Id,
                    CreatedAt = now
                };
                _db.AttemptCompetencies.Add(attemptCompetency);
            }

            attemptCompetency.Score = score;
            attemptCompetency.Level = ToCompetencyLever(score);
        }
    }

    private async Task ApplyUserCompetencySignalsAsync(
        Attempt attempt,
        IReadOnlyDictionary<string, int> competencyTotals,
        IReadOnlyDictionary<string, Competency> competencyEntities,
        DateTimeOffset now)
    {
        if (competencyTotals.Count == 0)
        {
            return;
        }

        foreach (var (code, delta) in competencyTotals)
        {
            var competency = competencyEntities[code];

            var profile = await _db.UserCompetencies
                .FirstOrDefaultAsync(uc => uc.UserId == attempt.UserId && uc.CompetencyId == competency.Id);

            if (profile is null)
            {
                profile = new UserCompetency
                {
                    Id = Guid.NewGuid(),
                    UserId = attempt.UserId,
                    CompetencyId = competency.Id,
                    CreatedAt = now
                };
                _db.UserCompetencies.Add(profile);
            }

            profile.Score += delta;
            profile.Level = ToCompetencyLever(profile.Score);
            profile.UpdatedAt = now;
        }
    }

    private static CompetencyLever ToCompetencyLever(int score) => score switch
    {
        >= 2 => CompetencyLever.StrongSide,
        <= -1 => CompetencyLever.DevelopmentZone,
        _ => CompetencyLever.Acceptable
    };

    private static string ToUserCompetencyLevel(CompetencyLever level) => level switch
    {
        CompetencyLever.StrongSide => "strength",
        CompetencyLever.DevelopmentZone => "development_area",
        _ => "stable"
    };

    private static AttemptStateResponse BuildStateResponse(Attempt attempt, ScenarioNode node)
    {
        var finished = attempt.LifecycleStatus == AttemptLifecycleStatus.Finished;

        AttemptNodeDto? nodeDto = finished
            ? null
            : new AttemptNodeDto(
                attempt.CurrentNodeId!,
                node.Type,
                node.Text,
                node.Choices?.Select(c => new AttemptChoiceDto(c.Id, c.Text)).ToList() ?? new List<AttemptChoiceDto>(),
                attempt.NodeDeadlineAt,
                node.TimerSeconds);


        return new AttemptStateResponse(
            attempt.Id,
            finished ? "finished" : "in_progress",
            ToResultStatusString(attempt.ResultStatus),
            attempt.CurrentSafety,
            attempt.CurrentLoyalty,
            finished,
            nodeDto);
    }

    private static string? ToResultStatusString(AttemptResultStatus? status) => status switch
    {
        AttemptResultStatus.Success => ScenarioResultStatuses.Success,
        AttemptResultStatus.Failed => ScenarioResultStatuses.Failed,
        AttemptResultStatus.CriticalFailure => ScenarioResultStatuses.CriticalFailure,
        _ => null
    };
}

internal static class StatusCodes
{
    public const int NotFound = 404;
    public const int Conflict = 409;
    public const int UnprocessableEntity = 422;
    public const int InternalServerError = 500;
}
