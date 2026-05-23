
using System.Text;
using ArtificialPirates;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using OpenAI.Assistants;
using OpenAI.Chat;
using OpenAI.Embeddings;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var OPENAI_KEY = config["OpenAI_Key"]!;
var AZURE_OPENAI_KEY = config["Azure_OpenAI_Key"]!;
var AZURE_OPENAI_ENDPOINT = config["Azure_OpenAI_Endpoint"]!;

// === OpenAI Chat ====
var openaiChatClient = new ChatClient("gpt-4o", OPENAI_KEY);
//await OpenAIChat(openaiChatClient);
async Task OpenAIChat(ChatClient chatClient)
{
    List<ChatMessage> messages =
    [
        new SystemChatMessage("You are a pirate's parrot mocking users."),
        new AssistantChatMessage("Arrr! What be ye wantin' to know? 🦜"),
    ];

    while (true)
    {
        Console.WriteLine($"\n🦜: {messages[^1].Content[0].Text}\n");

        Console.Write("You: ");
        var userInput = Console.ReadLine()!;
        if (string.IsNullOrEmpty(userInput)) { break; }

        messages.Add(new UserChatMessage(userInput));
        var response = await chatClient.CompleteChatAsync(messages);
        messages.Add(new AssistantChatMessage(response.Value.Content[0].Text));
    }
}

// === Azure OpenAI Chat ====
//await AzureOpenAIChat();
async Task AzureOpenAIChat()
{
    var azureClient = new AzureOpenAIClient(
        new Uri(AZURE_OPENAI_ENDPOINT),
        new AzureKeyCredential(AZURE_OPENAI_KEY));
    await OpenAIChat(azureClient.GetChatClient("gpt-4o"));
}

// === Streaming ===
//await Streaming(openaiChatClient);
async Task Streaming(ChatClient chatClient)
{
    List<ChatMessage> messages =
    [
        new SystemChatMessage("You are a pirate's parrot mocking users."),
        new AssistantChatMessage("Arrr! What be ye wantin' to know? 🦜"),
    ];

    Console.WriteLine($"\n🦜: {messages[^1].Content[0].Text}\n");
    while (true)
    {
        Console.Write("You: ");
        var userInput = Console.ReadLine()!;
        if (string.IsNullOrEmpty(userInput)) { break; }

        messages.Add(new UserChatMessage(userInput));
        var messageBuilder = new StringBuilder();
        Console.Write("\n🦜: ");
        await foreach (var update in chatClient.CompleteChatStreamingAsync(messages))
        {
            foreach (ChatMessageContentPart updatePart in update.ContentUpdate)
            {
                messageBuilder.Append(updatePart.Text);
                Console.Write(updatePart.Text);
            }
        }
        Console.WriteLine();

        messages.Add(new AssistantChatMessage(messageBuilder.ToString()));
    }
}

// === Assistant API ===
#pragma warning disable OPENAI001
//await AssistantAPI(new AssistantClient(OPENAI_KEY));
async Task AssistantAPI(AssistantClient assistantClient)
{
    async Task<string?> GetAssistantByName(string name)
    {
        await foreach (var assistant in assistantClient.GetAssistantsAsync())
        {
            if (assistant.Name == name) { return assistant.Id; }
        }

        return null;
    }

    async Task<int> GetNumberOfMessagesInThread(string threadId)
    {
        int count = 0;
        await foreach (var message in assistantClient.GetMessagesAsync(threadId)) { count++; }
        return count;
    }

    var assistantId = await GetAssistantByName("PirateParrot");
    if (assistantId is not null)
    {
        // Delete assistant and recreate it
        await assistantClient.DeleteAssistantAsync(assistantId);
    }

    var assistant = await assistantClient.CreateAssistantAsync("gpt-4o", new AssistantCreationOptions
    {
        Name = "PirateParrot",
        Description = "A pirate's parrot that mocks users.",
        Instructions = "You are a pirate's parrot mocking users.",
    });

    const string INITIAL_MESSAGE = "Arrr! What be ye wantin' to know? 🦜";
    var options = new ThreadCreationOptions { InitialMessages = { INITIAL_MESSAGE } };
    var thread = await assistantClient.CreateThreadAsync(options);

    Console.WriteLine($"\n🦜: {INITIAL_MESSAGE}\n");
    while (true)
    {
        Console.Write("You: ");
        var userInput = Console.ReadLine()!;
        if (string.IsNullOrEmpty(userInput)) { break; }

        await assistantClient.CreateMessageAsync(thread.Value.Id, [userInput]);
        Console.Write("\n🦜: ");
        await foreach (StreamingUpdate update in assistantClient.CreateRunStreamingAsync(thread.Value.Id, assistant.Value.Id))
        {
            if (update is MessageContentUpdate contentUpdate)
            {
                Console.Write(contentUpdate.Text);
            }
        }
        Console.WriteLine();

        Console.WriteLine($"\nMessages in thread: {await GetNumberOfMessagesInThread(thread.Value.Id)}\n");
    }
}

// === Embeddings ===
await EmbeddingsAPI(
    new EmbeddingClient("text-embedding-3-large", OPENAI_KEY),
    new ChatClient("gpt-4o", OPENAI_KEY));
async Task EmbeddingsAPI(EmbeddingClient embeddingClient, ChatClient chatClient)
{
    var piratesEmbeddings = await embeddingClient.GenerateEmbeddingsAsync(Pirates.FamousPirates.Select(pirate => pirate.ToString()));

    Console.Write("Describe the pirate: ");
    // Try:
    // * Who is dressed in green?
    // * Who has a dagger?
    // * Anybody wearing an eye patch?
    var query = Console.ReadLine()!;
    Console.WriteLine();

    var queryEmbedding = await embeddingClient.GenerateEmbeddingAsync(query);

    var mostSimilarPirates = piratesEmbeddings.Value
        .Select((qe, ix) => (Similarity: MathHelpers.DotProduct(queryEmbedding.Value.Vector.Span, qe.Vector.Span), Index: ix))
        .OrderByDescending(similarity => similarity.Similarity)
        .Select(similarity => Pirates.FamousPirates[similarity.Index])
        .Take(3);

    List<ChatMessage> messages =
    [
        new SystemChatMessage($"""
            You are a pirate's parrot answering questions about pirates. User describe
            what pirate they are looking. Make suggestions based on the
            pirate descriptions provided below. ONLY use the provided descriptions.
            Do NOT use other information sources. Answer the user in the language
            of his question.

            If you cannot generate a meaningful answer based on the given description,
            write an excuse.

            Write like a pirate's parrot in a funny way with emojis and pirate slang.

            ===========
            {string.Join("\n\n--------\n\n", mostSimilarPirates)}
            ===========
            """),
        new UserChatMessage(query),
    ];

    var response = await chatClient.CompleteChatAsync(messages);
    Console.WriteLine($"{response.Value.Content[0].Text}\n");
}
