using EmailNotificationService.Models;
using Microsoft.Extensions.Options;

namespace EmailNotificationService.Abstractions;

public abstract class EmailSenderBase(IOptions<EmailSettings> emailSettings) : IEmailSender
{
	protected EmailSettings EmailSettings { get; } = emailSettings.Value;

	public abstract Task<SendEmailResult> SendAsync(Email email);

	protected static void ValidateEmail(Email email)
	{
		ArgumentNullException.ThrowIfNull(email);

		if (string.IsNullOrEmpty(email.ToEmail))
		{
			throw new ArgumentException("To email address is required to send an email");
		}
	}
}
