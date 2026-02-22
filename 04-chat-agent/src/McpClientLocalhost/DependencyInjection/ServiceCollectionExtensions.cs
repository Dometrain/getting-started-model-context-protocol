using McpClientLocalhost.Agent;
using McpClientLocalhost.Configuration;

namespace McpClientLocalhost.DependencyInjection;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcpClientInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.SectionName));
        services.Configure<AgentOptions>(configuration.GetSection(AgentOptions.SectionName));

        services.AddSingleton<IConversationStore, InMemoryConversationStore>();

        services.AddScoped<IAgentService, AgentService>();

        return services;
    }
}

