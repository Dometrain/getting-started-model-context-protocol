using McpClientAuthenticated.Dtos;

namespace McpClientAuthenticated.Agent;

public interface IAgentService
{
    Task<ChatResponse> ProcessAsync(ChatRequest request, CancellationToken cancellationToken = default);
}