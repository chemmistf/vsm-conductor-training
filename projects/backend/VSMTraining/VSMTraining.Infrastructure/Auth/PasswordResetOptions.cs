namespace VSMTraining.Infrastructure.Auth;

public class PasswordResetOptions
{
    public const string SectionName = "PasswordReset";
    public string FrontendBaseUrl { get; set; } = "http://localhost:5173";
}