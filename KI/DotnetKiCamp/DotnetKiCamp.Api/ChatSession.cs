using System.Runtime.CompilerServices;
using System.Text;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace DotnetKiCamp;

public interface IChatSession
{
    Guid Id { get; }

    void AddUserMessage(string message);
    bool LastMessageIsFromUser { get; }
    Task<string> RunBasic(CancellationToken cancellationToken);
    IAsyncEnumerable<string> Run(CancellationToken cancellationToken);
    IEnumerable<ChatMessage> GetMessageHistory();
}

public partial class ChatSession(OpenAIClient client, IOptions<OpenAISettings> settings, ILogger logger) : IChatSession
{
    const string ErrorMessage = "I'm sorry, I'm having trouble understanding you right now. Please try again later.";

    /// <summary>
    /// Lock to ensure that <see cref="Options"/> is only accessed for writing by one thread at a time.
    /// </summary>
    /// <remarks>
    /// Necessary because Azure API model classes are not thread-safe. For more details
    /// see https://devblogs.microsoft.com/azure-sdk/lifetime-management-and-thread-safety-guarantees-of-azure-sdk-net-clients/#thread-safety-models-are-not-thread-safe.
    /// </remarks>
    private readonly object OptionsLock = new();

    public ChatCompletionsOptions Options { get; private set; } = new(
        settings.Value.ModelName,
        [
            new ChatRequestSystemMessage(settings.Value.SystemPrompt)
        ]);

    public Guid Id { get; } = Guid.NewGuid();

    public void AddUserMessage(string message)
    {
        lock (OptionsLock) { Options.Messages.Add(new ChatRequestUserMessage(message)); }
    }

    public bool LastMessageIsFromUser
    {
        get => Options.Messages.LastOrDefault() is ChatRequestUserMessage;
    }
    
    public async Task<string> RunBasic(CancellationToken cancellationToken)
    {
        var completions = await client.GetChatCompletionsAsync(Options, cancellationToken);
        var content = completions.Value.Choices[0].Message.Content;
        lock (OptionsLock) { Options.Messages.Add(new ChatRequestAssistantMessage(content)); }
        return content;
    }

    public async IAsyncEnumerable<string> Run([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var completions = await client.GetChatCompletionsStreamingAsync(Options, cancellationToken);

        var messageBuilder = new StringBuilder();
        await foreach (var c in completions)
        {
            if (!string.IsNullOrEmpty(c.ContentUpdate))
            {
                yield return c.ContentUpdate;
                messageBuilder.Append(c.ContentUpdate);
            }
        }

        lock (OptionsLock) { Options.Messages.Add(new ChatRequestAssistantMessage(messageBuilder.ToString())); }
    }

    public async IAsyncEnumerable<string> RunWithFunctions(IStreamProcessor streamProcessor, 
        IAiFunctions aiFunctions, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // Make sure that the functions are added to the chat completions options
        aiFunctions.EnsureFunctionsAreInCompletionsOptions(Options);

        bool repeat;
        do
        {
            repeat = false;

            streamProcessor.Reset();
            await foreach (var msg in streamProcessor.ProcessResponseStream(Options, cancellationToken))
            {
                if (msg is ContentUpdate cu)
                {
                    yield return cu.Content;
                } else if (msg is FunctionCall fc)
                {
                    LogReceivedFunctionCall(logger, fc.Id, fc.Name, fc.ArgumentJson.ToString());
                    yield return $"Let me call {fc.Name} for you...\n\n";

                    // We could now start to process the function call in a background
                    // task while waiting for further messages from OpenAI.
                }
            }

            // Add OpenAI response to the chat history
            lock (OptionsLock) { Options.Messages.Add(streamProcessor.BuildAssistantMessage()); }

            if (streamProcessor.HasFunctions)
            {
                // We could execute all functions is parallel, but for simplicity
                // we execute them one by one.
                foreach (var call in streamProcessor.FunctionCalls)
                {
                    var result = await aiFunctions.Execute(call);
                    lock (OptionsLock) { Options.Messages.Add(result); }
                }

                repeat = true;
            }
        }
        while (repeat);
    }

    public IEnumerable<ChatMessage> GetMessageHistory()
    {
        lock (OptionsLock)
        {
            return Options.Messages
                .Where(m => m is ChatRequestUserMessage or ChatRequestAssistantMessage)
                .Select(m => m switch
                    {
                        ChatRequestUserMessage um => new ChatMessage(Sender.User, um.Content),
                        ChatRequestAssistantMessage am => new ChatMessage(Sender.Assistant, am.Content),
                        _ => throw new InvalidOperationException("Unexpected message type, this should never happen"),
                    });
        }
    }

    [LoggerMessage(Level = LogLevel.Information,
        Message = "Received call {id} to {functionName} with arguments {arguments}")]
    public static partial void LogReceivedFunctionCall(
        ILogger logger, string? id, string functionName, string arguments);
}

public enum Sender
{
    User,
    Assistant
}

public record ChatMessage(Sender Sender, string Message);
