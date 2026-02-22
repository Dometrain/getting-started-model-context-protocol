namespace McpClientAuthenticated.Configuration;

public sealed class EntraOptions
{
    public const string SectionName = "Entra";

    public required string TenantId { get; set; }

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string Audience { get; set; }
}