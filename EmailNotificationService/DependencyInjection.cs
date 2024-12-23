using EmailNotificationService.Abstractions;
using EmailNotificationService.EmailSenders;
using EmailNotificationService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EmailNotificationService;

public static class DependencyInjection
{
	public static void AddEmailNotifications(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));

		services.AddTransient<EmailTemplateLoader>();
		services.AddTransient<EmailRenderer>();
		services.AddTransient<EmailService>();

		services.AddTransient<IEmailSender>(provider =>
		{
			var emailSettings = provider.GetRequiredService<IOptions<EmailSettings>>().Value;

			if (emailSettings.Enabled)
			{
				return emailSettings.EmailSendingMode switch
				{
					EmailSendingMode.Smtp => provider.GetRequiredService<SmtpEmailSender>(),
					EmailSendingMode.SendGrid => provider.GetRequiredService<SendGridEmailSender>(),
					_ => throw new ArgumentOutOfRangeException(nameof(emailSettings.EmailSendingMode)),
				};
			}

			return null;
		});

		services.AddTransient<SendGridEmailSender>();
		services.AddTransient<SmtpEmailSender>();
	}
}
