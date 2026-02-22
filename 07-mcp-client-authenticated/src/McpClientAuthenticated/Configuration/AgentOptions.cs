namespace McpClientAuthenticated.Configuration;

public sealed class AgentOptions
{
    public const string SectionName = "Agent";

    public string SystemPrompt { get; set; } = "You are a helpful AI assistant.";

    public int TimeoutSeconds { get; set; } = 120;
}
