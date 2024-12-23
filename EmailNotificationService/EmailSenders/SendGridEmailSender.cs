using System.Net.Http.Json;
using EmailNotificationService.Abstractions;
using EmailNotificationService.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;

namespace EmailNotificationService.EmailSenders;

public class SendGridEmailSender : EmailSenderBase
{
	private readonly ILogger<SendGridEmailSender> _logger;
	private readonly EmailSettings _emailSettings;

	public SendGridEmailSender(IOptions<EmailSettings> emailSettings, ILogger<SendGridEmailSender> logger)
		: base(emailSettings)
	{
		_logger = logger;
		_emailSettings = emailSettings.Value;

		ArgumentNullException.ThrowIfNull(_emailSettings.SendGridSettings.SendGridApiKey);
	}

	public override async Task<SendEmailResult> SendAsync(Email email)
	{
		if (!_emailSettings.Enabled)
		{
			_logger.LogInformation("Not sending email as email sending is not enabled in settings.");
			return null;
		}

		ValidateEmail(email);

		var client = new SendGridClient(_emailSettings.SendGridSettings.SendGridApiKey);

		var toAddress = new EmailAddress(email.ToEmail, email.ToName);
		var fromAddress = new EmailAddress(email.FromEmail, email.FromName);

		var message = MailHelper.CreateSingleEmail(
			fromAddress,
			toAddress,
			email.Subject,
			email.TextContent,
			email.HtmlContent
		);

		try
		{
			var response = await client.SendEmailAsync(message);

			if (response.IsSuccessStatusCode)
			{
				return new SendEmailResult(true);
			}

			if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
			{
				_logger.LogError(
					"SendGrid forbidden error when attempting to send email to '{Email}' from '{FromEmail}'. "
						+ "This usually happens when the email address the email is being sent from is not an "
						+ "email under the registered domain for SendGrid.",
					email.ToEmail,
					email.FromEmail
				);
				return new SendEmailResult(false);
			}

			var errors = await response.Body.ReadFromJsonAsync<SendGridErrorResponse>();

			_logger.LogError(
				"SendGrid error when attempting to send email to {Email}: [{Field}] {Error}",
				email.ToEmail,
				errors?.FieldWithError,
				errors?.SendGridErrorMessage
			);

			return new SendEmailResult(false);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error when attempting to send email to {Email}", email.ToEmail);
			return new SendEmailResult(false);
		}
	}
}
