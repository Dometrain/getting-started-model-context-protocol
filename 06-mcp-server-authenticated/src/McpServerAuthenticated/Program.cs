using McpServerAuthenticated.Configuration;
using McpServerAuthenticated.Storage;
using McpServerAuthenticated.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<StorageOptions>(
    builder.Configuration.GetSection(StorageOptions.SectionName));

builder.Services.AddSingleton<StorageClient>();
builder.Services.AddSingleton<TodoListTools>();

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
    .WithTools<TodoListTools>();

var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://+:{port}");


app.UseAuthentication();
app.UseAuthorization();
app.MapMcp("/mcp").RequireAuthorization("McpToolsScope");

app.Run();
