using McpClientLocalhost.Agent;
using McpClientLocalhost.Configuration;
using McpClientLocalhost.Mcp;

namespace McpClientLocalhost.DependencyInjection;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcpClientInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.SectionName));
        services.Configure<AgentOptions>(configuration.GetSection(AgentOptions.SectionName));
        services.Configure<McpOptions>(configuration.GetSection(McpOptions.SectionName));

        services.AddHttpClient<IMcpConnection, McpConnection>()
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


