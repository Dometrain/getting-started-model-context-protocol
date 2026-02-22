using Azure;
using Azure.Data.Tables;

public enum TodoStatus
{
    Pending,
    Completed,
    Cancelled
}

public class TodoEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "default";
    public string RowKey { get; set; } = default!;
    public string Text { get; set; } = default!;
    public DateTimeOffset CreatedUtc { get; set; }

    public ETag ETag { get; set; }
    public DateTimeOffset? Timestamp { get; set; }

    public TodoStatus Status { get; set; } = TodoStatus.Pending;
    public int Priority { get; set; } = 0;
    public DateTimeOffset? DueDate { get; set; }
    public string? Notes { get; set; }

    public string Id => RowKey;
}