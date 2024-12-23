// ReSharper disable ClassNeverInstantiated.Global

using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using EmailNotificationService.Abstractions;
using EmailNotificationService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmailNotificationService.EmailSenders;

public class SmtpEmailSender : EmailSenderBase
{
	private readonly ILogger<SmtpEmailSender> _logger;

	public SmtpEmailSender(IOptions<EmailSettings> emailSettings, ILogger<SmtpEmailSender> logger)
		: base(emailSettings)
	{
		_logger = logger;

		if (EmailSettings.SmtpSettings is null)
		{
			throw new ArgumentException("SMTP settings cannot be empty");
		}

		if (string.IsNullOrEmpty(EmailSettings.SmtpSettings.Host) || EmailSettings.SmtpSettings.Port == 0)
		{
			throw new ArgumentException("The SMTP host and port must be provided in SMTP settings");
		}
	}

	public override async Task<SendEmailResult> SendAsync(Email email)
	{
		if (!EmailSettings.Enabled)
		{
			return null;
		}

		ValidateEmail(email);

		using var client = new SmtpClient(EmailSettings.SmtpSettings.Host, EmailSettings.SmtpSettings.Port)
		{
			EnableSsl = EmailSettings.SmtpSettings.UseSsl,
		};

		if (
			!string.IsNullOrEmpty(EmailSettings.SmtpSettings.Username)
			&& !string.IsNullOrEmpty(EmailSettings.SmtpSettings.Password)
		)
		{
			client.Credentials = new NetworkCredential(
				EmailSettings.SmtpSettings.Username,
				EmailSettings.SmtpSettings.Password
			);
		}

		var toAddress = new MailAddress(email.ToEmail, email.ToName);
		var fromAddress = new MailAddress(email.FromEmail, email.FromName);

		var message = new MailMessage(fromAddress, toAddress)
		{
			Subject = email.Subject,
			SubjectEncoding = Encoding.UTF8,
		};

		message.AlternateViews.Add(
			AlternateView.CreateAlternateViewFromString(email.TextContent, Encoding.UTF8, MediaTypeNames.Text.Plain)
		);
		message.AlternateViews.Add(
			AlternateView.CreateAlternateViewFromString(email.HtmlContent, Encoding.UTF8, MediaTypeNames.Text.Html)
		);

		try
		{
			await client.SendMailAsync(message);
			return new SendEmailResult(true);
		}
		catch (InvalidOperationException ex)
		{
			_logger.LogError(ex, "Invalid SMTP configuration is preventing email being send to {Email}", email.ToEmail);
			return new SendEmailResult(false);
		}
		catch (SmtpFailedRecipientException ex)
		{
			_logger.LogWarning(ex, "Unable to send email message to recipient {Email}", email.ToEmail);
			return new SendEmailResult(false);
		}
		catch (SmtpException ex)
		{
			_logger.LogError(ex, "SMTP error when attempting to send email to {Email}", email.ToEmail);
			return new SendEmailResult(false);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error when attempting to send email to {Email}", email.ToEmail);
			return new SendEmailResult(false);
		}
	}
}
