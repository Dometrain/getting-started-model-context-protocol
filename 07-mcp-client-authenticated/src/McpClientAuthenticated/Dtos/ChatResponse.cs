namespace McpClientAuthenticated.Dtos;

public sealed record ChatResponse
{
    public required string Answer { get; init; }

    public required string ConversationId { get; init; }

}