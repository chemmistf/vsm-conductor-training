namespace VSMTraining.Application.Auth;

public interface IEmailSender
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetUrl);  
}