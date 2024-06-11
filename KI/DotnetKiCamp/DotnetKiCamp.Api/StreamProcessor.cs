using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Azure.AI.OpenAI;

namespace DotnetKiCamp;

public interface IStreamProcessor
{
    string Content { get; }
    IReadOnlyList<FunctionCall> FunctionCalls { get; }
    bool HasContent { get; }
    bool HasFunctions { get; }
    ChatRequestAssistantMessage BuildAssistantMessage();
    IAsyncEnumerable<StreamingResult> ProcessResponseStream(ChatCompletionsOptions chatCompletionsOptions, CancellationToken cancellationToken = default);
    void Reset();
}

/// <summary>
/// Processes streamed responses (content or function calls) from the OpenAI API.
/// </summary>
public class StreamProcessor(OpenAIClient client) : IStreamProcessor
{
    private readonly StringBuilder ContentBuilder = new();
    private readonly List<FunctionCall> FunctionCallList = [];

    public string Content => ContentBuilder.ToString();

    public IReadOnlyList<FunctionCall> FunctionCalls => FunctionCallList;

    public bool HasContent => ContentBuilder.Length > 0;

    public bool HasFunctions => FunctionCallList.Count > 0;

    public ChatRequestAssistantMessage BuildAssistantMessage()
    {
        if (HasContent)
        {
            return new ChatRequestAssistantMessage(Content);
        }
        else
        {
            var m = new ChatRequestAssistantMessage("");
            foreach (var call in FunctionCalls)
            {
                m.ToolCalls.Add(new ChatCompletionsFunctionToolCall(call.Id, call.Name, call.ArgumentJson.ToString()));
            }

            return m;
        }
    }

    /// <summary>
    /// Processes the response stream from the OpenAI API and returns 
    /// the results as a stream of <see cref="StreamingResult"/>.
    /// </summary>
    /// <remarks>
    /// After this method has been called, the <see cref="Content"/> 
    /// and <see cref="FunctionCalls"/> properties will contain the 
    /// content and function calls from the response stream.
    /// </remarks>
    public async IAsyncEnumerable<StreamingResult> ProcessResponseStream(
        ChatCompletionsOptions chatCompletionsOptions,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var completions = await client.GetChatCompletionsStreamingAsync(chatCompletionsOptions, cancellationToken);

        var currentFunctionCall = new FunctionCallBuilder();
        var currentToolIndex = -1;
        await foreach (var c in completions)
        {
            if (c.ToolCallUpdate != null)
            {
                var update = c.ToolCallUpdate as StreamingFunctionToolCallUpdate;
                Debug.Assert(update != null);
                if (update.ToolCallIndex != currentToolIndex && currentToolIndex != -1)
                {
                    // We have a new tool call, so add the previous one to the list
                    var fc = currentFunctionCall.ToFunctionCall();
                    FunctionCallList.Add(fc);
                    yield return fc;
                    currentFunctionCall = new();
                }

                currentToolIndex = update.ToolCallIndex;

                // Update the current function call
                if (currentFunctionCall.Id == null && !string.IsNullOrEmpty(update.Id)) { currentFunctionCall.Id = update.Id; }
                if (currentFunctionCall.Name == null && !string.IsNullOrEmpty(update.Name)) { currentFunctionCall.Name = update.Name; }
                currentFunctionCall.ArgumentJson.Append(update.ArgumentsUpdate);
            }
            else if (!string.IsNullOrEmpty(c.ContentUpdate))
            {
                yield return new ContentUpdate(c.ContentUpdate);
                ContentBuilder.Append(c.ContentUpdate);
            }
            else
            {
                continue;
            }
        }

        // Add the last function call to the list
        if (!currentFunctionCall.IsEmpty)
        {
            var fc = currentFunctionCall.ToFunctionCall();
            FunctionCallList.Add(fc);
            yield return fc;
        }
    }

    public void Reset()
    {
        ContentBuilder.Clear();
        FunctionCallList.Clear();
    }
}

/// <summary>
/// Helper class for building a function call from streaming responses.
/// </summary>
file class FunctionCallBuilder
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public StringBuilder ArgumentJson { get; private set; } = new();

    public bool IsEmpty => Id is null && Name is null && ArgumentJson.Length == 0;

    public FunctionCall ToFunctionCall() => new(Id!, Name!, ArgumentJson.ToString());
}

/// <summary>
/// Represents a result from the OpenAI API that is streamed to the client.
/// </summary>
public abstract record StreamingResult();

/// <summary>
/// Represents a complete function call that has been streamed from the OpenAI API.
/// </summary>
public record FunctionCall(string Id, string Name, string ArgumentJson) : StreamingResult;

/// <summary>
/// Represents a content update that has been streamed from the OpenAI API.
/// </summary>
public record ContentUpdate(string Content) : StreamingResult;
