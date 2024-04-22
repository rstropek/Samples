using System.Collections.Concurrent;
using System.Diagnostics;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace DotnetKiCamp;

/// <summary>
/// Abstraction for <see cref="ChatManager"/> to make it easier to test.
/// </summary>
public interface IChatManager
{
    IChatSession CreateSession();
    IChatSession? GetSession(Guid sessionId);
}

/// <summary>
/// Manages chat sessions
/// </summary>
/// <seealso cref="ChatSession" />
public class ChatManager(OpenAIClient client, IOptions<OpenAISettings> settings, ILogger<ChatManager> logger) : IChatManager
{
    /// <summary>
    /// Collection of chat sessions
    /// </summary>
    /// <remarks>
    /// In this example, we store the chat sessions in memory. In a real-world application,
    /// you would likely store the chat sessions in a database or a distributed cache.
    /// </remarks>
    private readonly ConcurrentDictionary<Guid, ChatSession> Sessions = [];

    public IChatSession CreateSession()
    {
        var session = new ChatSession(client, settings, logger);
        var added = Sessions.TryAdd(session.Id, session);
        Debug.Assert(added, "Created a new guid, add should always succeed");
        return session;
    }

    public IChatSession? GetSession(Guid sessionId)
    {
        if (Sessions.TryGetValue(sessionId, out var session))
        {
            return session;
        }

        return null;
    }
}
