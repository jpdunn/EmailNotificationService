using System.Reflection;

namespace EmailNotificationService;

public class EmailTemplateLoader
{
	private readonly Assembly _executingAssembly = Assembly.GetExecutingAssembly();

	public async Task<string> LoadTemplateAsync(string fileName)
	{
		var path = Path.Combine(
			Path.GetDirectoryName(_executingAssembly.Location),
			Path.Combine("EmailTemplates", fileName)
		);

		await using var stream = File.OpenRead(path);

		if (stream == null)
		{
			throw new ArgumentException($"Template not found: {fileName}", nameof(fileName));
		}

		using var reader = new StreamReader(stream);
		return await reader.ReadToEndAsync();
	}
}
