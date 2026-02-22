namespace McpServerToolingWithIntent.Storage;

public class TodoListResult
{
    public required List<TodoEntity> Items { get; set; }
    public string? NextCursor { get; set; }
    public int TotalCount { get; set; }
}

public class TodoCandidatesResult
{
    public required List<TodoEntity> Candidates { get; set; }
    public int Count => Candidates.Count;
}