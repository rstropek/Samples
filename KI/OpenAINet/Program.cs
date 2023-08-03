#pragma warning disable CS8321

using System.Diagnostics;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
// using Azure.Core;
// using Azure.Identity;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var endpoint = configuration["Azure:OpenAIEndpoint"]!;
var key = configuration["Azure:OpenAIKey"]!;
var deployment = configuration["Azure:DeploymentName"]!;
var weatherApiKey = configuration["WeatherApiKey"]!;

var client = new OpenAIClient(new Uri(endpoint),
    new AzureKeyCredential(key));
//new DefaultAzureCredential());

Console.WriteLine("=== About Vienna ===");
//await AboutVienna(client, deployment);

Console.WriteLine("=== About Vienna (streamed) ===");
//await AboutViennaStreamed(client, deployment);

Console.WriteLine("=== What to do in Vienna ===");
await WhatToDoInVienna(client, deployment, weatherApiKey);

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

static async Task WhatToDoInVienna(OpenAIClient client, string deployment, string weatherApiKey)
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
        new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }),
    };

    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        Messages =
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant."),
            new ChatMessage(ChatRole.User,
                """
                Suggest three things to do in Vienna. Consider the current weather.
                """)
        },
        Functions = { getWeatherFuntionDefinition }
    };

    var response = await client.GetChatCompletionsAsync(deployment, chatCompletionsOptions);

    ChatChoice responseChoice = response.Value.Choices[0];
    if (responseChoice.FinishReason == CompletionsFinishReason.FunctionCall)
    {
        chatCompletionsOptions.Messages.Add(responseChoice.Message);

        if (responseChoice.Message.FunctionCall.Name == "get_current_weather")
        {
            string args = responseChoice.Message.FunctionCall.Arguments;
            var getWeatherParameter = JsonSerializer.Deserialize<GetWeatherParameter>(args, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            Debug.Assert(getWeatherParameter != null);
            Debug.Assert(!string.IsNullOrEmpty(getWeatherParameter.Location));

            var api = new WeatherApi(weatherApiKey);
            var weather = await api.GetCurrentWeather(getWeatherParameter.Location);

            var functionResponseMessage = new ChatMessage(ChatRole.Function, weather) { Name = "get_current_weather" };
            chatCompletionsOptions.Messages.Add(functionResponseMessage);

            response = await client.GetChatCompletionsAsync(deployment, chatCompletionsOptions);
            Console.WriteLine(response.Value.Choices[0].Message.Content);
            Console.WriteLine();
        }
    }
}

record GetWeatherParameter(string Location);
