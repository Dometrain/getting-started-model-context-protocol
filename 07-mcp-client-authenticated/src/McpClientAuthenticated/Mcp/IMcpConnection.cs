using ModelContextProtocol.Client;

namespace McpClientAuthenticated.Mcp;

public interface IMcpConnection : IAsyncDisposable
{
    McpClient Client { get; }
    Task EnsureConnectedAsync(CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}
