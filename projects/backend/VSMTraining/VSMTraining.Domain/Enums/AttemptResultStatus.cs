namespace VSMTraining.Domain.Enums;

public enum AttemptResultStatus
{
    InProgress,
    Passed,
    FailedSafety,
    FailedCriticalError,
    Timeout
}