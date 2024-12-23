using System.Runtime.Serialization;

namespace EmailNotificationService;

[Serializable]
public class InvalidTemplatePlaceholderException : Exception
{
	public string Placeholder { get; }

	public InvalidTemplatePlaceholderException(string placeholder, string message)
		: base(message)
	{
		Placeholder = placeholder;
	}

	protected InvalidTemplatePlaceholderException(SerializationInfo info, StreamingContext context)
		: base(info, context) { }
}
