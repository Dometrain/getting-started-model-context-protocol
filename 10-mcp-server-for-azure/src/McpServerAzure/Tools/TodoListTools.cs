using System.ComponentModel;
using McpServerAzure.Storage;
using ModelContextProtocol.Server;

namespace McpServerAzure.Tools;

[McpServerToolType]
public class TodoListTools
{
    private readonly StorageClient _storageClient;

    public TodoListTools(StorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    [McpServerTool, Description("Adds a new todo item to the list. Returns the created todo with its ID.")]
    public async Task<TodoEntity> AddTodo([Description("The text of the todo item")] string text)
    {
        return await _storageClient.AddTodoAsync(text);
    }

    [McpServerTool, Description("Gets a specific todo item by its ID.")]
    public async Task<TodoEntity?> GetTodo([Description("The unique identifier of the todo item")] string id)
    {
        return await _storageClient.GetTodoAsync(id);
    }

    [McpServerTool, Description("Marks a todo item as completed. Returns the updated todo.")]
    public async Task<TodoEntity> CompleteTodo([Description("The unique identifier of the todo item to complete")] string id)
    {
        return await _storageClient.CompleteTodoAsync(id);
    }

    [McpServerTool, Description("Reopens a completed todo item, changing its status back to pending. Returns the updated todo.")]
    public async Task<TodoEntity> ReopenTodo([Description("The unique identifier of the todo item to reopen")] string id)
    {
        return await _storageClient.ReopenTodoAsync(id);
    }

    [McpServerTool, Description("Sets the priority of a todo item. Higher numbers indicate higher priority (0 = normal, positive = higher, negative = lower). Returns the updated todo.")]
    public async Task<TodoEntity> SetPriority(
        [Description("The unique identifier of the todo item")] string id,
        [Description("The priority value (0 = normal, positive = higher priority, negative = lower priority)")] int priority)
    {
        return await _storageClient.SetPriorityAsync(id, priority);
    }

    [McpServerTool, Description("Sets the due date for a todo item. Returns the updated todo.")]
    public async Task<TodoEntity> SetDueDate(
        [Description("The unique identifier of the todo item")] string id,
        [Description("The due date and time in ISO 8601 format (e.g., 2024-12-31T23:59:59Z)")] DateTimeOffset dueDate)
    {
        return await _storageClient.SetDueDateAsync(id, dueDate);
    }

    [McpServerTool, Description("Adds a note to a todo item. Notes are appended to any existing notes. Returns the updated todo.")]
    public async Task<TodoEntity> AddNote(
        [Description("The unique identifier of the todo item")] string id,
        [Description("The note text to add to the todo item")] string note)
    {
        return await _storageClient.AddNoteAsync(id, note);
    }

    [McpServerTool, Description("Searches for todo items matching a query string. Supports filtering by status and pagination.")]
    public async Task<TodoListResult> FindTodos(
        [Description("The search query to match against todo text and notes")] string query,
        [Description("Optional: Filter by status (Pending, Completed, Cancelled)")] TodoStatus? status = null,
        [Description("Maximum number of results to return (default: 50)")] int limit = 50,
        [Description("Optional: Cursor for pagination (from previous FindTodos result)")] string? cursor = null)
    {
        return await _storageClient.FindTodosAsync(query, status, limit, cursor);
    }

    [McpServerTool, Description("Gets a paginated and filtered list of todo items. Supports filtering by status, priority, and due date. Returns results with pagination cursor.")]
    public async Task<TodoListResult> GetTodoList(
        [Description("Optional: Filter by status (Pending, Completed, Cancelled)")] TodoStatus? status = null,
        [Description("Optional: Filter by exact priority value")] int? priority = null,
        [Description("Optional: Only return todos with due date before this date/time (ISO 8601 format)")] DateTimeOffset? dueBefore = null,
        [Description("Maximum number of results to return (default: 50)")] int limit = 50,
        [Description("Optional: Cursor for pagination (from previous GetTodoList result)")] string? cursor = null)
    {
        return await _storageClient.ListTodosAsync(status, priority, dueBefore, limit, cursor);
    }

    [McpServerTool, Description("DANGEROUS: Permanently deletes a todo item from the list. This action cannot be undone. Returns true if deleted, false if not found.")]
    public async Task<bool> DeleteTodo([Description("The unique identifier of the todo item to delete")] string id)
    {
        return await _storageClient.DeleteTodoAsync(id);
    }

}
