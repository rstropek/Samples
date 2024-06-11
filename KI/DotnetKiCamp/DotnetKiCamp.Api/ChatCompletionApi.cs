using System.Text.Json.Serialization;

namespace DotnetKiCamp;

/// <summary>
/// Implements the chat completion API for this sample
/// </summary>
public static class ChatCompletionsApi
{
    public static RouteGroupBuilder MapChatCompletions(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/chat");

        // This is the endpoint that creates a new chat session
        group.MapPost("/complete", (IChatManager chatManager) =>
        {
            var session = chatManager.CreateSession();
            return Results.Created((Uri?)null, new SessionCreationResponse(session.Id));
        });

        // This endpoint adds a message to the chat session
        group.MapPost("/complete/{sessionId}/messages", (IChatManager chatManager, Guid sessionId, AddMessageRequest message) =>
        {
            if (chatManager.GetSession(sessionId) is not ChatSession session) { return Results.NotFound(); }
            session.AddUserMessage(message.Message);
            return Results.Created();
        });

        // This endpoint triggers AI processing of the messages in the chat session.
        group.MapGet("/complete/{sessionId}/run-basic", async (IChatManager chatManager, Guid sessionId, CancellationToken cancellationToken) =>
        {
            if (chatManager.GetSession(sessionId) is not ChatSession session)
            {
                return Results.NotFound();
            }

            if (!session.LastMessageIsFromUser)
            {
                return Results.NoContent();
            }

            var content = await session.RunBasic(cancellationToken);
            return Results.Ok(content);
        });

        // This endpoint triggers AI processing of the messages in the chat session.
        // It returns a Server-Sent Events stream of messages from the AI.
        group.MapGet("/complete/{sessionId}/run", async (HttpContext ctx, IChatManager chatManager, Guid sessionId,
            IStreamProcessor streamProcessor, IAiFunctions aiFunctions, CancellationToken cancellationToken) =>
        {
            if (chatManager.GetSession(sessionId) is not ChatSession session)
            {
                ctx.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            if (!session.LastMessageIsFromUser)
            {
                // If the last message was from the assistant, this is a reconnect request
                // triggered by the Server-Sent Events reconnect logic. We don't need to send
                // the last message again, so we just return a 204 No Content response.
                // The client will stop trying to reconnect.
                ctx.Response.StatusCode = StatusCodes.Status204NoContent;
                return;
            }

            ctx.Response.Headers.Append("Content-Type", "text/event-stream");
            //await foreach (var msg in session.Run(cancellationToken))
            await foreach (var msg in session.RunWithFunctions(streamProcessor, aiFunctions, cancellationToken))
            {
                await ctx.Response.WriteAsync($"data: ", cancellationToken: cancellationToken);
                await ctx.Response.WriteAsync(msg.Replace("\n", "\\n"), cancellationToken: cancellationToken);
                await ctx.Response.WriteAsync($"\n\n", cancellationToken: cancellationToken);
                await ctx.Response.Body.FlushAsync(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        });

        // This endpoint returns the message history of the chat session
        group.MapGet("/complete/{sessionId}/messages", (IChatManager chatManager, Guid sessionId) =>
        {
            if (chatManager.GetSession(sessionId) is not ChatSession session) { return Results.NotFound(); }
            return Results.Ok(session.GetMessageHistory());
        });

        return group;
    }
    public record SessionCreationResponse(Guid SessionId);

    public record AddMessageRequest(string Message);
}

[JsonSourceGenerationOptions(UseStringEnumConverter = true)]
[JsonSerializable(typeof(ChatCompletionsApi.SessionCreationResponse))]
[JsonSerializable(typeof(ChatCompletionsApi.AddMessageRequest))]
[JsonSerializable(typeof(IEnumerable<ChatMessage>))]
[JsonSerializable(typeof(Sender))]
[JsonSerializable(typeof(string))]
internal partial class ChatCompletionsApiSerializerContext : JsonSerializerContext { }
