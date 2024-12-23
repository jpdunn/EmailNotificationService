namespace EmailNotificationService.Models;

public class EmailSettings
{
	public bool Enabled { get; set; }

	public EmailSendingMode EmailSendingMode { get; set; }

	public SmtpSettings SmtpSettings { get; set; }

	public SendGridSettings SendGridSettings { get; set; }
}
