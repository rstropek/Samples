#pragma warning disable CS8321

using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var endpoint = configuration["Azure:OpenAIEndpoint"]!;
var key = configuration["Azure:OpenAIKey"]!;
var deployment = configuration["Azure:DeploymentName"]!;

var client = new OpenAIClient(new Uri(endpoint),
    new AzureKeyCredential(key));
//new DefaultAzureCredential());

Console.WriteLine("=== About Vienna ===");
//await AboutVienna(client, deployment);

Console.WriteLine("=== About Vienna (streamed) ===");
await AboutViennaStreamed(client, deployment);

static async Task AboutVienna(OpenAIClient client, string deployment)
{
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        Messages =
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant."),
            new ChatMessage(ChatRole.User,
                """
                Tell me a little bit about Vienna. Answer in the German "Wienerisch"-Slang.
                Address me with the typical "Oida" greeting.
                """)
        },
        MaxTokens = 1000
    };

    var response = await client.GetChatCompletionsAsync(deployment, chatCompletionsOptions);

    Console.WriteLine(response.Value.Choices[0].Message.Content);
    Console.WriteLine();
}

static async Task AboutViennaStreamed(OpenAIClient client, string deployment)
{
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        Messages =
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant."),
            new ChatMessage(ChatRole.User,
                """
                Tell me a little bit about Vienna. Answer in the German "Wienerisch"-Slang.
                Address me with the typical "Oida" greeting.
                """)
        },
        MaxTokens = 1000
    };

    var response = await client.GetChatCompletionsStreamingAsync(deployment, chatCompletionsOptions);
    
    using var streamingChatCompletions = response.Value;
    await foreach (var choice in streamingChatCompletions.GetChoicesStreaming())
    {
        await foreach (var message in choice.GetMessageStreaming())
        {
            Console.Write(message.Content);
        }
        Console.WriteLine();
    }
}

static async Task WhatToDoInVienna(OpenAIClient client, string deployment)
{
    var getWeatherFuntionDefinition = new FunctionDefinition()
    {
        Name = "get_current_weather",
        Description = "Get the current weather in a given location",
        Parameters = BinaryData.FromObjectAsJson(
        new
        {
            Type = "object",
            Properties = new
            {
                Location = new
                {
                    Type = "string",
                    Description = "The city and state, e.g. San Francisco, CA",
                }
            },
            Required = new[] { "location" },
        },
        new JsonSerializerOptions() {  PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
    };
}