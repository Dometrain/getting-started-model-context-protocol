using System.ComponentModel;
using System.Text;
using System.Text.Json;
using McpServerResources.Storage;
using ModelContextProtocol.Server;
using NJsonSchema;
using NJsonSchema.Generation;

namespace McpServerResources.Resources;

[McpServerResourceType]
public class TodoResources
{
    private readonly StorageClient _storageClient;

    public TodoResources(StorageClient storageClient)
    {
        _storageClient = storageClient;
    }
    
    private static readonly SystemTextJsonSchemaGeneratorSettings SchemaSettings = new SystemTextJsonSchemaGeneratorSettings
    {
        AlwaysAllowAdditionalObjectProperties = false,
        DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
        SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }
    };

    [McpServerResource(UriTemplate = "todo://schema", Name = "Todo System Schema", MimeType = "application/schema+json")]
    [Description("JSON schema definition for the Todo system, including fields and status types.")]
    public Task<string> GetSchema()
    {
        var schema = JsonSchema.FromType<TodoEntity>(SchemaSettings);
        return Task.FromResult(schema.ToJson());
    }

    [McpServerResource(UriTemplate = "todo://stats/dashboard", Name = "Todo Statistics Dashboard", MimeType = "text/markdown")]
    [Description("A real-time dashboard showing high-level statistics about the todo system.")]
    public async Task<string> GetStatsDashboard()
    {
        var result = await _storageClient.ListTodosAsync(limit: 1000);
         var sb = new StringBuilder();
        sb.AppendLine("# Todo System Dashboard");
        sb.AppendLine();
        sb.AppendLine($"**Total Todos:** {result.TotalCount}");
        sb.AppendLine();

        var pendingCount = result.Items.Count(t => t.Status == TodoStatus.Pending);
        var completedCount = result.Items.Count(t => t.Status == TodoStatus.Completed);
        var cancelledCount = result.Items.Count(t => t.Status == TodoStatus.Cancelled);

        sb.AppendLine("## Status Breakdown");
        sb.AppendLine($"- **Pending:** {pendingCount}");
        sb.AppendLine($"- **Completed:** {completedCount}");
        sb.AppendLine($"- **Cancelled:** {cancelledCount}");
        sb.AppendLine();

        var highPriorityCount = result.Items.Count(t => t.Priority > 0);
        var overdueCount = result.Items.Count(t => t.DueDate.HasValue && t.DueDate < DateTimeOffset.UtcNow && t.Status == TodoStatus.Pending);

        sb.AppendLine("## Attention Needed");
        sb.AppendLine($"- **High Priority:** {highPriorityCount}");
        sb.AppendLine($"- **Overdue:** {overdueCount}");
        sb.AppendLine();
        
        sb.AppendLine($"*Last Updated: {DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss} UTC*");

        return sb.ToString();
    }
}