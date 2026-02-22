using McpClientLocalhost.Agent;
using McpClientLocalhost.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace McpClientLocalhost.Endpoints;

public static class ChatEndpoints
{
    private static async Task<IResult> HandleChatAsync(
        [FromBody] ChatRequest request,
        [FromServices] IAgentService agentService,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await agentService.ProcessAsync(request, cancellationToken);
            return Results.Ok(response);
        }
        catch (OperationCanceledException)
        {
            return Results.StatusCode(StatusCodes.Status408RequestTimeout);
        }
        catch (Exception)
        {
            return Results.Problem(
                detail: "An error occurred while processing your request.",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api")
            .WithTags("Chat");

        group.MapPost("/chat", HandleChatAsync)
            .WithName("Chat")
            .WithDescription("Process a user message and return the agent's response using Microsoft Agent Framework")
            .Produces<ChatResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError);

        group.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
            .WithName("Health");

        return endpoints;
    }
}