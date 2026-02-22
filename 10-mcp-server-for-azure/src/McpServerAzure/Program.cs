using McpServerAzure.Configuration;
using McpServerAzure.Resources;
using McpServerAzure.Storage;
using McpServerAzure.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                ForwardedHeaders.XForwardedProto |
                                ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});


builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection(StorageOptions.SectionName));

builder.Services.AddSingleton<StorageClient>();
builder.Services.AddSingleton<TodoListTools>();
builder.Services.AddSingleton<TodoResources>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("McpToolsScope", policy =>
        policy.RequireScope("mcp.tools").RequireAuthenticatedUser());
});


builder.Services
    .AddMcpServer()
    .WithHttpTransport()
    .WithResources<TodoResources>()
    .WithTools<TodoListTools>();

var app = builder.Build();

app.UseForwardedHeaders();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://+:{port}");


app.UseAuthentication();
app.UseAuthorization();
app.MapMcp("/mcp").RequireAuthorization("McpToolsScope");

app.Run();
