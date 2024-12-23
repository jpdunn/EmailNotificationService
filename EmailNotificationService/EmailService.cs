using EmailNotificationService.Abstractions;
using EmailNotificationService.Models;
using Microsoft.Extensions.Logging;

namespace EmailNotificationService;

public class EmailService(
	EmailRenderer emailRenderer,
	IEmailSender emailSender,
	ILogger<EmailService> logger
)
{
	public async Task<(Email, SendEmailResult)> SendEmailAsync(NotificationType notificationType, EmailContentData emailContentData)
	{
		// Determine template based on notification type.
		ITemplate? template = notificationType switch
		{
			NotificationType.WelcomeNewUser => new WelcomeEmailTemplate(emailContentData),
			_ => null,
		};

		if (template == null)
		{
			logger.LogError(
				"Unable to send email because notification type {NotificationType} could not be found",
				notificationType
			);
			return (null, null);
		}

		var email = await emailRenderer.RenderAsync(template, emailContentData);

		var result = await emailSender.SendAsync(email);

		return (email, result);
	}
}
