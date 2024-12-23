// ReSharper disable ClassNeverInstantiated.Global

namespace EmailNotificationService.Models;

public class SendGridSettings
{
	public string SendGridApiKey { get; set; } = string.Empty;

	public string SendGridHost { get; set; } = string.Empty;
}
