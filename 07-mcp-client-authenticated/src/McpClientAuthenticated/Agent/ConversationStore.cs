using System.Collections.Concurrent;
using Microsoft.Agents.AI;

namespace McpClientAuthenticated.Agent;

public interface IConversationStore
{
    AgentThread? GetThread(string conversationId);
    void SaveThread(string conversationId, AgentThread thread);
}

public sealed class InMemoryConversationStore : IConversationStore
{
    private readonly ConcurrentDictionary<string, AgentThread> _threads = new();

    public AgentThread? GetThread(string conversationId)
    {
        _threads.TryGetValue(conversationId, out var thread);
        return thread;
    }

    public void SaveThread(string conversationId, AgentThread thread)
    {
        _threads[conversationId] = thread;
    }
}