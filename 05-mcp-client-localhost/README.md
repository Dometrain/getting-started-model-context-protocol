# MCP Client for .NET 10

Builds on section 04 (chat agent) by adding **MCP client integration**. Connects to a localhost MCP server (e.g., section 02 or 03), fetches tools from the server, and exposes them to the AI agent so it can invoke them during conversation.

## Before you run

1. **OpenAI API Key**: Replace `<OPENAI_API_KEY>` in `src/McpClientLocalhost/appsettings.json` or `.vscode/launch.json` (for F5 debugging). Alternatively use User Secrets or environment variable `OpenAI__ApiKey`.