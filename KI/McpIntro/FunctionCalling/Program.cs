using Microsoft.Extensions.Configuration;
using OpenAI.Responses;
using FunctionCalling;

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var client = new OpenAIResponseClient("gpt-5", config["OPENAI_KEY"]);
var systemPrompt = await File.ReadAllTextAsync("system-prompt.md");

Console.WriteLine("🤖: How can I help?");

string? previousResponseId = null;

while (true)
{
    Console.Write("\nYou (empty to quit): ");
    var userMessage = Console.ReadLine()!;
    if (string.IsNullOrEmpty(userMessage))
    {
        break;
    }

    Console.Write("\n🤖: ");
    previousResponseId = await client.CreateAssistantResponseAsync(userMessage, systemPrompt, previousResponseId);
    Console.WriteLine();
}
