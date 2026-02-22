namespace McpClientLocalhost.Configuration;

public sealed class McpOptions
{
    public const string SectionName = "Mcp";

    public required string BaseUrl { get; set; }

    public int TimeoutSeconds { get; set; } = 30;
}