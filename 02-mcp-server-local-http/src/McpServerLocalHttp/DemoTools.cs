using System.ComponentModel;
using ModelContextProtocol.Server;

[McpServerToolType]
public class DemoTools
{
    [McpServerTool, Description("Echoes the input message back to the caller.")]
    public static string Echo([Description("The message to echo")] string message)
    {
        return message;
    }

    [McpServerTool, Description("Returns the current UTC date and time in ISO 8601 format.")]
    public static string UtcNow()
    {
        return DateTime.UtcNow.ToString("o");
    }
}
