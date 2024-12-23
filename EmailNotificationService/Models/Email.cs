namespace EmailNotificationService.Models;

/// <summary>
/// A record that holds the header and content of an email.
/// </summary>
/// <param name="ToName">The name of the recipient.</param>
/// <param name="ToEmail">The email address of the recipient.</param>
/// <param name="FromName">The name of the sender.</param>
/// <param name="FromEmail">The email address of the sender.</param>
/// <param name="Subject">The subject line of the email.</param>
/// <param name="HtmlContent">The HTML content of the email.</param>
/// /// <param name="TextContent">The plain text content of the email.</param>
public record Email(
	string ToName,
	string ToEmail,
	string FromName,
	string FromEmail,
	string Subject,
	string HtmlContent,
	string TextContent
);
