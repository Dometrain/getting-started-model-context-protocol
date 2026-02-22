namespace McpClientAuthenticated.Configuration;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAI";

    public required string ApiKey { get; set; }

    public string Model { get; set; } = "gpt-5-mini";

}
