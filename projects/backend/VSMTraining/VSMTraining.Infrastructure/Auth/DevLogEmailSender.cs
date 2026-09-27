using Microsoft.Extensions.Logging;
using VSMTraining.Application.Auth;

namespace VSMTraining.Infrastructure.Auth;

public class DevLogEmailSender : IEmailSender
{
    private readonly ILogger<DevLogEmailSender> _logger;

    public DevLogEmailSender(ILogger<DevLogEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendPasswordResetEmailAsync(string toEmail, string resetUrl)
    {
        _logger.LogInformation("Password reset link for {Email}: {ResetUrl}", toEmail, resetUrl);
        return Task.CompletedTask;
    }
}