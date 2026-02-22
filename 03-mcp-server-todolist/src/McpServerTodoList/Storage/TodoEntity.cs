using Azure;
using Azure.Data.Tables;

public class TodoEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "default";
    public string RowKey { get; set; } = default!;
    public string Text { get; set; } = default!;
    public DateTimeOffset CreatedUtc { get; set; }

    public ETag ETag { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}