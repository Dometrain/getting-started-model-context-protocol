using McpClientLocalhost.Configuration;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Client;

namespace McpClientLocalhost.Mcp;

public sealed class McpConnection(
    HttpClient httpClient,
    IOptions<McpOptions> options,
    ILogger<McpConnection> logger) : IMcpConnection
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly McpOptions _options = options.Value;
    private readonly ILogger<McpConnection> _logger = logger;
    private McpClient? _client;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);
    private bool _disposed;

    public McpClient Client => _client 
        ?? throw new InvalidOperationException("Connection not established. Call EnsureConnectedAsync first.");

    public bool IsConnected => _client is not null;

    public async Task EnsureConnectedAsync(CancellationToken cancellationToken = default)
    {
        if (_client is not null)
            return;

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            _logger.LogInformation("Connecting to MCP server at {BaseUrl}", _options.BaseUrl);

            var transport = new HttpClientTransport(
                new HttpClientTransportOptions
                {
                    Endpoint = new Uri(_options.BaseUrl),
                    Name = "McpClient",
                },
                _httpClient);

            _client = await McpClient.CreateAsync(transport, cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully connected to MCP server");
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, 
                "HTTP error connecting to MCP server at {BaseUrl}. Status: {StatusCode}, Message: {Message}",
                _options.BaseUrl,
                httpEx.Data.Contains("StatusCode") ? httpEx.Data["StatusCode"] : "Unknown",
                httpEx.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to MCP server at {BaseUrl}", _options.BaseUrl);
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (_client is not null)
        {
            await _client.DisposeAsync();
            _client = null;
        }

        _connectionLock.Dispose();
    }
}
