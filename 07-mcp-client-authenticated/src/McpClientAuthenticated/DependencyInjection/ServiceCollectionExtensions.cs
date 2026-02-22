using McpClientAuthenticated.Agent;
using McpClientAuthenticated.Configuration;
using McpClientAuthenticated.Mcp;

namespace McpClientAuthenticated.DependencyInjection;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcpClientInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.SectionName));
        services.Configure<AgentOptions>(configuration.GetSection(AgentOptions.SectionName));
        services.Configure<McpOptions>(configuration.GetSection(McpOptions.SectionName));
        services.Configure<EntraOptions>(configuration.GetSection(EntraOptions.SectionName));
        services.AddTransient<McpAuthenticationHandler>();
        services.AddHttpContextAccessor();
        services.AddHttpClient<IMcpConnection, McpConnection>()
            .AddHttpMessageHandler<McpAuthenticationHandler>()        
            .ConfigureHttpClient((sp, client) =>
            {
                var options = configuration.GetSection(McpOptions.SectionName).Get<McpOptions>();
                if (options is not null)
                {
                    client.BaseAddress = new Uri(options.BaseUrl);
                    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
                }
            });

        services.AddSingleton<IConversationStore, InMemoryConversationStore>();

        services.AddScoped<IAgentService, AgentService>();

        return services;
    }
}
