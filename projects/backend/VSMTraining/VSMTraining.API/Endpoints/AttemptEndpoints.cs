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
            .WithTags("Игровые попытки")
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
            .WithName("StartAttempt")
            .WithSummary("Начать игровую попытку")
            .WithDescription("Создаёт новую попытку прохождения сценария для текущего пользователя и возвращает первый узел. " +
                             "Если scenarioId не передан, используется демонстрационный сценарий. " +
                             "В ответе находятся идентификатор попытки, текущие шкалы безопасности и лояльности, " +
                             "а также доступные варианты выбора и дедлайн узла, если он ограничен по времени.")
            .Produces<AttemptStateResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized);

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
            .WithName("ChooseAttemptOption")
            .WithSummary("Сделать выбор в попытке")
            .WithDescription("Фиксирует выбранный вариант текущего узла, пересчитывает шкалы и переводит попытку на следующий узел. " +
                             "Если выбор завершает сценарий, попытка помечается как завершённая и начисляется XP. " +
                             "Если дедлайн узла уже истёк, сервер вместо выбора применяет timeout-исход узла. " +
                             "choiceId должен принадлежать текущему узлу этой попытки.")
            .Produces<AttemptStateResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict)
            .Produces<ApiErrorResponse>(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized);

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
            .WithName("TimeoutAttempt")
            .WithSummary("Применить истечение времени узла")
            .WithDescription("Завершает текущий узел по timeout-правилам сценария: применяет изменения шкал и компетенций, " +
                             "а затем возвращает следующий узел или финальное состояние попытки. " +
                             "Вызов допустим только для активного узла с настроенным таймером после фактического истечения дедлайна.")
            .Produces<AttemptStateResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized);

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
            .WithName("GetAttemptResult")
            .WithSummary("Получить результат попытки")
            .WithDescription("Возвращает итог завершённой попытки: статус результата, текст финала, начисленный XP, " +
                             "изменения шкал безопасности и лояльности, критические ошибки, итоговые компетенции " +
                             "и подробную хронологию действий. Результат нельзя получить, пока попытка ещё выполняется.")
            .Produces<ResultResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces<ApiErrorResponse>(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status401Unauthorized);
    }

    private static bool TryGetUserId(ClaimsPrincipal principal, out Guid userId)
    {
        var value = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        return Guid.TryParse(value, out userId);
    }
}
