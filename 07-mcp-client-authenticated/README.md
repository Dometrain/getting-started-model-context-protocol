# MCP Client for .NET 10

Builds on section 05 by adding **authentication**. Connects to the authenticated MCP server (section 06) and passes a bearer token when calling tools. Supports optional Entra ID user auth for the chat UI.

## Before you run

1. **appsettings.json** and **.vscode/launch.json** (for F5 debugging): Replace the placeholders:
   - `<AZURE_CLIENT_ID>` in `Mcp.Scope` - your MCP Server's app registration Client ID.
   - `<OPENAI_API_KEY>` - your OpenAI API key.
   - For Entra ID auth (optional): `<AZURE_TENANT_ID>`, `<FRONTEND_CLIENT_ID>`, `<ENTRA_CLIENT_SECRET>` in Entra settings.

2. **get-user-token.ps1**: Pass your MCP Client API's Client ID when running the script (see below).

```powershell
.\scripts\get-user-token.ps1 -ClientId "<mcp-client-api-client-id>"
```

## THIS DOES WORK!!

There is nothing wrong with the code.  If it doesn't work try stuff like...

1. Make sure the authenticated server (06) is running when you start this
2. Make sure you do a clean `dotnet build`
3. Put a breakpoint inside this lambda `async (JsonElement arguments, CancellationToken ct) => await InvokeMcpToolAsync(toolName, arguments.GetRawText(), ct),` it WILL hit it
4. Get a fresh token
5. Just try different stuff but I promise this code works when connected to the spc-06 server
6. It works even without the EntraID settings in appsettings.json

