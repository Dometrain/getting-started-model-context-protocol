using System.Collections.Frozen;
using System.Text.Json;
using McpClientAuthenticated.Configuration;
using McpClientAuthenticated.Dtos;
using McpClientAuthenticated.Mcp;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

namespace McpClientAuthenticated.Agent;


public sealed class AgentService(
    IOptions<AgentOptions> agentOptions,
    IOptions<OpenAiOptions> openAiOptions,
    IMcpConnection mcpConnection,
    IConversationStore conversationStore,
    ILogger<AgentService> logger) : IAgentService
{
    private readonly AgentOptions _agentOptions = agentOptions.Value;
    private readonly OpenAiOptions _openAiOptions = openAiOptions.Value;
    private readonly IMcpConnection _mcpConnection = mcpConnection;
    private readonly IConversationStore _conversationStore = conversationStore;
    private readonly ILogger<AgentService> _logger = logger;
    private AIAgent? _agent;
    private FrozenDictionary<string, ModelContextProtocol.Client.McpClientTool> _mcpToolRegistry = FrozenDictionary<string, ModelContextProtocol.Client.McpClientTool>.Empty;


    private async Task<AIAgent> GetOrCreateAgentAsync(CancellationToken cancellationToken)
    {
        if (_agent is not null)
            return _agent;

        var openAiClient = new OpenAIClient(_openAiOptions.ApiKey);
        var chatClient = openAiClient.GetChatClient(_openAiOptions.Model);
        await _mcpConnection.EnsureConnectedAsync(cancellationToken);

        var mcpTools = await _mcpConnection.Client.ListToolsAsync(
            options: null,
            cancellationToken: cancellationToken);

        _logger.LogDebug(
            "Retrieved {ToolCount} tools from MCP server: {ToolNames}",
            mcpTools.Count,
            string.Join(", ", mcpTools.Select(t => t.Name)));

        _mcpToolRegistry = mcpTools.ToFrozenDictionary(t => t.Name);

        var agentTools = new List<AITool>();

        foreach (var mcpTool in mcpTools)
        {
            try
            {
                var tool = CreateAgentTool(mcpTool);
                agentTools.Add(tool);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create Agent Framework tool for MCP tool: {ToolName}", mcpTool.Name);
            }
        }


        _agent = chatClient.CreateAIAgent(
            instructions: _agentOptions.SystemPrompt,
            tools: agentTools.ToArray());

        return _agent;
    }

    public async Task<Dtos.ChatResponse> ProcessAsync(ChatRequest request, CancellationToken cancellationToken = default)
    {
        var conversationId = request.ConversationId ?? Guid.NewGuid().ToString("N");

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(_agentOptions.TimeoutSeconds));
        var timeoutToken = cts.Token;

        try
        {
            var agent = await GetOrCreateAgentAsync(timeoutToken);

            var thread = _conversationStore.GetThread(conversationId);
            if (thread == null)
            {
                thread = agent.GetNewThread();
                _conversationStore.SaveThread(conversationId, thread);
            }

            var runOptions = new AgentRunOptions();
            var result = await agent.RunAsync(request.Message, thread, options: runOptions, cancellationToken: timeoutToken);

            var responseText = result.Text ?? "I apologize, but I wasn't able to generate a response.";

            _logger.LogInformation(
                "Agent completed. Response: {Response}",
                responseText);

            return new Dtos.ChatResponse
            {
                Answer = responseText,
                ConversationId = conversationId,

            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing chat request");
            throw;
        }
    }
    private static string ExtractMcpResultContent(ModelContextProtocol.Protocol.CallToolResult result)
    {
        if (result.Content == null || result.Content.Count == 0)
            return string.Empty;

        return string.Join("\n", result.Content
            .Select(c => c.ToString())
            .Where(s => !string.IsNullOrEmpty(s)));

    }

    private async Task<string> InvokeMcpToolAsync(string toolName, string jsonArgs, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Invoking MCP tool: {ToolName} with arguments: {Arguments}",
                toolName,
                jsonArgs);

            if (!_mcpToolRegistry.TryGetValue(toolName, out var mcpTool))
            {
                var error = $"Error: Tool '{toolName}' not found";
                _logger.LogWarning(error);
                return error;
            }

            await _mcpConnection.EnsureConnectedAsync(cancellationToken);

            var argsDict = string.IsNullOrWhiteSpace(jsonArgs) 
                ? [] 
                : JsonDocument.Parse(jsonArgs).RootElement.EnumerateObject()
                    .ToDictionary(prop => prop.Name, prop => (object?)prop.Value.ToString());

            var mcpResult = await _mcpConnection.Client.CallToolAsync(
                toolName,
                argsDict,
                options: null,
                cancellationToken: cancellationToken);

            var result = ExtractMcpResultContent(mcpResult);

            _logger.LogInformation(
                "Tool {ToolName} invoked. Result: {Result}",
                toolName,
                result);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invoking MCP tool {ToolName}", toolName);
            return $"Error: {ex.Message}";
        }
    }

    private AITool CreateAgentTool(ModelContextProtocol.Client.McpClientTool mcpTool)
    {
        var toolName = mcpTool.Name;
        var toolDescription = mcpTool.Description ?? $"MCP tool: {toolName}";
        
        if (mcpTool.JsonSchema.ValueKind != JsonValueKind.Undefined && mcpTool.JsonSchema.ValueKind != JsonValueKind.Null)
        {
            var schemaJson = JsonSerializer.Serialize(mcpTool.JsonSchema, new JsonSerializerOptions { WriteIndented = false });
            toolDescription += $"\nParameters (JSON Schema): {schemaJson}";
        }
        
        var tool = AIFunctionFactory.Create(
            async (AIFunctionArguments? args, CancellationToken ct) =>
            {
                var jsonArgs = args is null or { Count: 0 }
                    ? "{}"
                    : JsonSerializer.Serialize(args);
                return await InvokeMcpToolAsync(toolName, jsonArgs, ct);
            },
            new AIFunctionFactoryOptions
            {
                Name = toolName,
                Description = toolDescription
            });
        
        _logger.LogInformation(
            "Created Agent Framework tool for MCP tool: {ToolName}",
            toolName);
        
        return tool;
    }



}

