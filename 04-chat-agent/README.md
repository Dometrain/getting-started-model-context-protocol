# Chat Agent (.NET 10)

A conversational AI chat API using the .NET Agent Framework and OpenAI. This section builds the **chat agent foundation** that will have MCP tool integration added in the next section.

**No MCP yet** - the agent uses an empty tool set (`tools: []`). Section 05 adds the MCP client connection.

## What this section covers

- Web API with chat endpoints
- OpenAI integration via the Agent Framework
- Conversation/thread persistence (in-memory store)
- CORS configured for the frontend (localhost:5173)

## Before you run

1. **OpenAI API Key**: Replace `<OPENAI_API_KEY>` in `src/McpClientLocalhost/appsettings.json` or `.vscode/launch.json` (for F5 debugging). Alternatively use User Secrets or environment variable `OpenAI__ApiKey`.
