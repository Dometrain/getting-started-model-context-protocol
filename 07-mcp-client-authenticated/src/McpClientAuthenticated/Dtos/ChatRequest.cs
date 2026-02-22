namespace McpClientAuthenticated.Dtos;

public sealed record ChatRequest
{
    public required string Message { get; init; }

    public string? ConversationId { get; init; }
}