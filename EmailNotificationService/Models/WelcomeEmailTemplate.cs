using EmailNotificationService.Abstractions;

namespace EmailNotificationService.Models;

public class WelcomeEmailTemplate(EmailContentData contentData) : ITemplate
{
	public string Name { get; set; } = "Welcome to the fun!";

	public NotificationType Type { get; set; } = NotificationType.WelcomeNewUser;

	public string Subject { get; set; } = "We're glad to have you onboard";

	public string HtmlContentFileName { get; set; } = "WelcomeNewUser.mjml";

	public string TextContentFileName { get; set; } = "WelcomeNewUser.txt";

	public Dictionary<string, dynamic> Parameters { get; set; } = new()
	{
		{ "FirstName", contentData.ToCustomerName },
		{ "CompanyName", contentData.FromUsername },
		{ "UserName", contentData.CustomerUsername },
		{ "ContactEmail", contentData.FromEmail },
	};
}
