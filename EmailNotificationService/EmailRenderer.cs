using System.Text.RegularExpressions;
using EmailNotificationService.Abstractions;
using EmailNotificationService.Models;
using Microsoft.Extensions.Logging;
using Mjml.Net;

namespace EmailNotificationService;

public class EmailRenderer(EmailTemplateLoader templateLoader, ILogger<EmailRenderer> logger)
{
	private readonly MjmlRenderer _mjmlRenderer = new MjmlRenderer();
	private readonly MjmlOptions _mjmlOptions = new MjmlOptions { Beautify = false };

	public async Task<Email> RenderAsync(ITemplate template, EmailContentData contentData)
	{
		var htmlTemplateContent = await templateLoader.LoadTemplateAsync(template.HtmlContentFileName);

		var textTemplateContent = await templateLoader.LoadTemplateAsync(template.TextContentFileName);

		try
		{
			var toEmail = contentData.ToEmailAddress;
			var subject = template.Subject;

			var fromEmail = contentData.FromEmail;
			var fromName = contentData.FromUsername;

			var (htmlContent, renderErrors) = await _mjmlRenderer.RenderAsync(
				ReplacePlaceholders(UnrollLoops(htmlTemplateContent, template), template).Trim(),
				_mjmlOptions
			);

			if (renderErrors.Any())
			{
				logger.LogError(
					"Error rendering email via MJML for {Template} template: {Errors}",
					template.Name,
					renderErrors
				);

				return null;
			}

			var textContent = ReplacePlaceholders(UnrollLoops(textTemplateContent, template), template).Trim();

			return new Email(
				contentData.ToCustomerName,
				toEmail,
				fromName,
				fromEmail,
				subject,
				htmlContent,
				textContent
			);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Error rendering email for {Template} template", template.Name);
			return null;
		}
	}

	private static string UnrollLoops(string input, ITemplate template)
	{
		// Look for any loops specified in the template with the format matching [[ForEach.Thing]] and
		// ending with [[/ForEach]] but ignoring the actual casing.
		var loops = new Regex(
			@"\[\[FOREACH\.([A-Z]+)\]\](.+)\[\[\/FOREACH\]\]",
			RegexOptions.IgnoreCase | RegexOptions.Singleline
		);

		var matchEvaluator = new MatchEvaluator(match =>
		{
			var name = match.Groups[1].Value;
			var contents = match.Groups[2].Value;

			if (!template.Parameters.TryGetValue(name.ToUpperInvariant(), out var parameter))
			{
				throw new InvalidTemplatePlaceholderException(
					$"ForEach.{name}",
					$"{name} is not a valid parameter for a loop's iterator"
				);
			}

			var count = parameter.Value.Length;
			return string.Concat(Enumerable.Repeat(contents, count));
		});

		return loops.Replace(input, matchEvaluator);
	}

	private static string ReplacePlaceholders(string input, ITemplate template)
	{
		// Search for any placeholders which are denoted by double square brackets (i.e. [[Parameter.ClientName]]).
		// We also don't care about casing for these placeholders.
		var placeholders = new Regex(@"\[\[([A-Z]+)\.([A-Z]+)\]\]", RegexOptions.IgnoreCase);

		var matchEvaluator = new MatchEvaluator(match =>
		{
			var placeholder = match.Groups[0].Value;
			var source = match.Groups[1].Value;
			var name = match.Groups[2].Value;

			switch (source.ToUpperInvariant())
			{
				case "PARAMETER":
					template.Parameters.TryGetValue(name, out var parameterValue);

					if (parameterValue == null)
					{
						throw new InvalidTemplatePlaceholderException(placeholder, "Parameter value is missing");
					}

					return ConvertTextToXmlSafe(parameterValue.ToString(), true);

				default:
					throw new InvalidTemplatePlaceholderException(
						placeholder,
						$"{source} is not a valid placeholder source"
					);
			}
		});

		return placeholders.Replace(input, matchEvaluator);
	}

	private static string ConvertTextToXmlSafe(string text, bool isForXml)
	{
		return !isForXml ? text : System.Security.SecurityElement.Escape(text);
	}
}
