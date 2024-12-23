using EmailNotificationService.Models;

namespace EmailNotificationService.Abstractions;

public interface IEmailSender
{
	Task<SendEmailResult> SendAsync(Email email);
}
