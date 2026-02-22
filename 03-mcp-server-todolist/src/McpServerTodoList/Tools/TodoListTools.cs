using System.ComponentModel;
using McpServerTodoList.Storage;
using ModelContextProtocol.Server;

namespace McpServerTodoList.Tools;

[McpServerToolType]
public class TodoListTools
{
    private readonly StorageClient _storageClient;

    public TodoListTools(StorageClient storageClient)
    {
        _storageClient = storageClient;
    }

    [McpServerTool, Description("Adds a new todo item to the list.")]
    public async Task AddTodo([Description("The text of the todo item")] string text)
    {
        await _storageClient.AddTodoAsync(text);
    }

    [McpServerTool, Description("Returns the current list of todo items.")]
    public async Task<List<TodoEntity>> GetTodoList()
    {
        return await _storageClient.ListTodosAsync();
    }

    [McpServerTool, Description("Deletes a todo item from the list.")]
    public async Task DeleteTodo([Description("The id of the todo item")] string id)
    {
        await _storageClient.DeleteTodoAsync(id);
    }
}