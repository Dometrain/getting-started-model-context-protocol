# Demo MCP Server (.NET 10)

MCP Server using **STDIO transport**. The Model Context Protocol SDK communicates via stdin/stdout - typically used when the client launches the server as a child process (e.g., Cursor, Claude Desktop).

## Testing with MCP Inspector

This server uses STDIO transport, so the MCP Inspector must launch it as a child process.

### Option 1: Direct Command Line

From the workspace root, run:

```bash
# Using the official MCP Inspector
npx @modelcontextprotocol/inspector@latest dotnet run --project src/McpServerStdio/McpServerStdio.csproj

# Or using MCP Jam Inspector
npx @mcpjam/inspector@latest dotnet run --project src/McpServerStdio/McpServerStdio.csproj
```

### Option 2: Using a Configuration File

Use the provided `mcp-config.json` file and run:

```bash
npx @modelcontextprotocol/inspector@latest --config mcp-config.json
```

Example `mcp-config.json`:
```json
{
  "mcpServers": {
    "demo-server": {
      "command": "dotnet",
      "args": ["run", "--project", "src/McpServerStdio/McpServerStdio.csproj"]
    }
  }
}
```

**Note:** Do NOT run the server separately (e.g., with F5). The Inspector must launch it to communicate via STDIO.
