using ModelContextProtocol.Client;

namespace McpClientLocalhost.Mcp;

public interface IMcpConnection : IAsyncDisposable
{
    McpClient Client { get; }
    Task EnsureConnectedAsync(CancellationToken cancellationToken = default);
    bool IsConnected { get; }
}
