namespace VSMTraining.Application.Attempts;

/// <summary>
/// Ошибка бизнес-правила runtime попытки.
/// </summary>
public class AttemptFlowException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public AttemptFlowException(string code, int statusCode, string message) : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}