using McpClientAuthenticated.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System.IdentityModel.Tokens.Jwt;

namespace McpClientAuthenticated.Mcp;

public sealed class McpAuthenticationHandler : DelegatingHandler
{
    private readonly EntraOptions _entraOptions;
    private readonly McpOptions _mcpOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<McpAuthenticationHandler> _logger;
    private IConfidentialClientApplication? _app;
    private readonly SemaphoreSlim _appLock = new(1, 1);

    public McpAuthenticationHandler(
        IOptions<EntraOptions> entraOptions,
        IOptions<McpOptions> mcpOptions,
        IHttpContextAccessor httpContextAccessor,
        ILogger<McpAuthenticationHandler> logger)
    {
        _entraOptions = entraOptions.Value;
        _mcpOptions = mcpOptions.Value;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private async Task<IConfidentialClientApplication> GetOrCreateAppAsync()
    {
        if (_app is not null)
            return _app;

        await _appLock.WaitAsync();
        try
        {
            _app = ConfidentialClientApplicationBuilder
                .Create(_entraOptions.ClientId)
                .WithClientSecret(_entraOptions.ClientSecret)
                .WithAuthority(AzureCloudInstance.AzurePublic, _entraOptions.TenantId)
                .Build();

            return _app;
        }
        finally
        {
            _appLock.Release();
        }
    }

    private string? GetUserToken()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) 
            return null;

        if (httpContext.User.Identity?.IsAuthenticated != true) 
            return null;

        var authHeader = httpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) 
            return null;

        return authHeader["Bearer ".Length..].Trim();
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userToken = GetUserToken();
            if (string.IsNullOrEmpty(userToken))
                throw new InvalidOperationException("No valid user token found in HttpContext.");

            var app = await GetOrCreateAppAsync();
            
            var userAssertion = new UserAssertion(userToken);
            var result = await app.AcquireTokenOnBehalfOf(
                    [_mcpOptions.Scope],
                    userAssertion)
                .ExecuteAsync(cancellationToken);

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
        }
        catch (MsalException ex)
        {
            _logger.LogError(ex, "MSAL error during OBO token exchange: {Message}", ex.Message);
            throw new InvalidOperationException($"Token exchange failed: {ex.ErrorCode}", ex);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during OBO token acquisition");
            throw new InvalidOperationException("Unexpected error acquiring token for MCP server", ex);
        }

        var response = await base.SendAsync(request, cancellationToken);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("MCP server returned {StatusCode} for {Method} {Uri}", 
                (int)response.StatusCode, request.Method, request.RequestUri);
        }
        
        return response;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _appLock.Dispose();
        }
        base.Dispose(disposing);
    }
}
