using McpServerAuthenticated.Configuration;
using Microsoft.Extensions.Options;
using Azure.Data.Tables;

namespace McpServerAuthenticated.Storage;

public sealed class StorageClient
{
    private readonly StorageOptions _storageOptions;
    private readonly TableClient _tableClient;

    public StorageClient(IOptions<StorageOptions> storageOptions)
    {
        _storageOptions = storageOptions.Value;

        _tableClient = CreateTableClient();
    }

    private TableClient CreateTableClient()
    {
        var connectionString = _storageOptions.ConnectionString;
        var tableName = "Todos";

        var client = new TableClient(connectionString, tableName);
        client.CreateIfNotExists();

        return client;
    }

    public async Task AddTodoAsync(string text)
    {
        var todo = new TodoEntity
        {
            PartitionKey = "default",
            RowKey = Guid.NewGuid().ToString(),
            Text = text,
            CreatedUtc = DateTimeOffset.UtcNow
        };

        await _tableClient.AddEntityAsync(todo);
    }


    public async Task<List<TodoEntity>> ListTodosAsync()
    {
        var results = new List<TodoEntity>();

        await foreach (var todo in _tableClient.QueryAsync<TodoEntity>(
            t => t.PartitionKey == "default"))
        {
            results.Add(todo);
        }

        return results
            .OrderBy(t => t.CreatedUtc)
            .ToList();
    }

    public async Task DeleteTodoAsync(string todoId)
    {
        await _tableClient.DeleteEntityAsync("default", todoId);
    }

}