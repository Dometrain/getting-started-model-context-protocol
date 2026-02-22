using McpServerAzure.Configuration;
using Microsoft.Extensions.Options;
using Azure.Data.Tables;

namespace McpServerAzure.Storage;

public sealed class StorageClient
{
    private readonly StorageOptions _storageOptions;
    private readonly TableClient _tableClient;
    private const string PartitionKey = "default";

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

    public async Task<TodoEntity> AddTodoAsync(string text)
    {
        var todo = new TodoEntity
        {
            PartitionKey = PartitionKey,
            RowKey = Guid.NewGuid().ToString(),
            Text = text,
            CreatedUtc = DateTimeOffset.UtcNow,
            Status = TodoStatus.Pending,
            Priority = 0
        };

        await _tableClient.AddEntityAsync(todo);
        return todo;
    }

    public async Task<TodoEntity?> GetTodoAsync(string todoId)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<TodoEntity>(PartitionKey, todoId);
            return response.Value;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<bool> DeleteTodoAsync(string todoId)
    {
        try
        {
            await _tableClient.DeleteEntityAsync(PartitionKey, todoId);
            return true;
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return false;
        }
    }

    public async Task<TodoEntity> CompleteTodoAsync(string todoId)
    {
        var todo = await GetTodoAsync(todoId);
        if (todo == null)
            throw new InvalidOperationException($"Todo with id '{todoId}' not found");

        todo.Status = TodoStatus.Completed;
        await _tableClient.UpdateEntityAsync(todo, todo.ETag, TableUpdateMode.Replace);
        
        return todo;
    }

    public async Task<TodoEntity> ReopenTodoAsync(string todoId)
    {
        var todo = await GetTodoAsync(todoId);
        if (todo == null)
            throw new InvalidOperationException($"Todo with id '{todoId}' not found");

        todo.Status = TodoStatus.Pending;
        await _tableClient.UpdateEntityAsync(todo, todo.ETag, TableUpdateMode.Replace);
        
        return todo;
    }

    public async Task<TodoEntity> SetPriorityAsync(string todoId, int priority)
    {
        var todo = await GetTodoAsync(todoId);
        if (todo == null)
            throw new InvalidOperationException($"Todo with id '{todoId}' not found");

        todo.Priority = priority;
        await _tableClient.UpdateEntityAsync(todo, todo.ETag, TableUpdateMode.Replace);
        
        return todo;
    }

    public async Task<TodoEntity> SetDueDateAsync(string todoId, DateTimeOffset dueDate)
    {
        var todo = await GetTodoAsync(todoId);
        if (todo == null)
            throw new InvalidOperationException($"Todo with id '{todoId}' not found");

        todo.DueDate = dueDate;
        await _tableClient.UpdateEntityAsync(todo, todo.ETag, TableUpdateMode.Replace);
        
        return todo;
    }

    public async Task<TodoEntity> AddNoteAsync(string todoId, string note)
    {
        var todo = await GetTodoAsync(todoId);
        if (todo == null)
            throw new InvalidOperationException($"Todo with id '{todoId}' not found");

        todo.Notes = string.IsNullOrWhiteSpace(todo.Notes) 
            ? note 
            : $"{todo.Notes}\n{note}";
        
        await _tableClient.UpdateEntityAsync(todo, todo.ETag, TableUpdateMode.Replace);
        
        return todo;
    }

    public async Task<TodoListResult> ListTodosAsync(
        TodoStatus? status = null,
        int? priority = null,
        DateTimeOffset? dueBefore = null,
        int limit = 50,
        string? cursor = null)
    {
        var results = new List<TodoEntity>();
        var query = _tableClient.QueryAsync<TodoEntity>(entity => entity.PartitionKey == PartitionKey);

        await foreach (var todo in query)
        {
            if (status.HasValue && todo.Status != status.Value)
                continue;
            
            if (priority.HasValue && todo.Priority != priority.Value)
                continue;
            
            if (dueBefore.HasValue && (!todo.DueDate.HasValue || todo.DueDate.Value > dueBefore.Value))
                continue;

            results.Add(todo);
        }

        var sorted = results
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTimeOffset.MaxValue)
            .ThenBy(t => t.CreatedUtc)
            .ToList();

        var startIndex = cursor != null && int.TryParse(cursor, out var idx) ? idx : 0;
        var paginated = sorted.Skip(startIndex).Take(limit).ToList();
        
        var nextCursor = startIndex + paginated.Count < sorted.Count 
            ? (startIndex + paginated.Count).ToString() 
            : null;

        return new TodoListResult
        {
            Items = paginated,
            NextCursor = nextCursor,
            TotalCount = sorted.Count
        };
    }

    public async Task<TodoListResult> FindTodosAsync(
        string query,
        TodoStatus? status = null,
        int limit = 50,
        string? cursor = null)
    {
        var allResults = new List<TodoEntity>();
        var tableQuery = _tableClient.QueryAsync<TodoEntity>(entity => entity.PartitionKey == PartitionKey);

        await foreach (var todo in tableQuery)
        {
            if (status.HasValue && todo.Status != status.Value)
                continue;

            var searchText = query.ToLowerInvariant();
            if (todo.Text.ToLowerInvariant().Contains(searchText) ||
                (todo.Notes != null && todo.Notes.ToLowerInvariant().Contains(searchText)))
            {
                allResults.Add(todo);
            }
        }

        var sorted = allResults
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTimeOffset.MaxValue)
            .ThenBy(t => t.CreatedUtc)
            .ToList();

        var startIndex = cursor != null && int.TryParse(cursor, out var idx) ? idx : 0;
        var paginated = sorted.Skip(startIndex).Take(limit).ToList();
        
        var nextCursor = startIndex + paginated.Count < sorted.Count 
            ? (startIndex + paginated.Count).ToString() 
            : null;

        return new TodoListResult
        {
            Items = paginated,
            NextCursor = nextCursor,
            TotalCount = sorted.Count
        };
    }


}