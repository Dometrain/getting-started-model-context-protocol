using ModelContextProtocol.Server;

[McpServerToolType]
public class DemoTools
{
    [McpServerTool]
    public static string Echo(string message)
    {
        return message;
    }

    [McpServerTool]
    public static string UtcNow()
    {
        return DateTime.UtcNow.ToString("o");
    }
}
