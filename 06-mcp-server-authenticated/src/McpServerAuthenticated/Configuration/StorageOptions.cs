namespace McpServerAuthenticated.Configuration;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    public required string ConnectionString { get; set; }
}
