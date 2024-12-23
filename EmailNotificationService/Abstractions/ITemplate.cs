using EmailNotificationService.Models;

namespace EmailNotificationService.Abstractions;

public interface ITemplate
{
	public string Name { get; set; }

	public NotificationType Type { get; set; }

	public string Subject { get; set; }

	public string HtmlContentFileName { get; set; }

	public string TextContentFileName { get; set; }

	public Dictionary<string, dynamic> Parameters { get; set; }
}
