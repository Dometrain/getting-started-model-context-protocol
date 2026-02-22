using McpClientLocalhost.Dtos;

namespace McpClientLocalhost.Agent;

public interface IAgentService
{
    Task<ChatResponse> ProcessAsync(ChatRequest request, CancellationToken cancellationToken = default);
}