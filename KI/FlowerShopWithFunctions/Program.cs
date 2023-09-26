using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

var orderedBouquets = new List<Bouquet>();

var config = new ConfigurationBuilder()
    .AddUserSecrets(typeof(Program).Assembly)
    .Build();

FunctionDefinition AddBouquetFunction = new()
{
    Name = "add_flower_bouquet_order_item",
    Description = """
        Adds a flower bouquet order item. Must be called whenever
        the user has finished configuring a flower bouquet.
        Can be called multiple times for the same order if the user
        wants to order multiple bouquets.

        The method returns all bouquets that have been ordered so far.
        """,
    Parameters = BinaryData.FromObjectAsJson(
            new
            {
                Type = "object",
                Properties = new
                {
                    BouquetSize = new
                    {
                        Type = "string",
                        Description = "The size of the bouquet. Can be 'small', 'medium', or 'large'.",
                    },
                    Price = new
                    {
                        Type = "number",
                        Description = "The price of the bouquet.",
                    },
                    Flowers = new
                    {
                        Type = "array",
                        Description = "Flowers in the bouquet",
                        Items = new
                        {
                            Type = "object",
                            Properties = new
                            {
                                FlowerType = new
                                {
                                    Type = "string",
                                    Description = "The type of flower (e.g. rose, lily, gerbera, etc.)",
                                },
                                Amount = new
                                {
                                    Type = "number",
                                    Description = "The amount of flowers of this type in the bouquet",
                                },
                            }
                        }
                    },
                },
                Required = new[] { "bouquetSize", "flowers" }
            },
            jsonOptions
        )
};

var completionOptions = new ChatCompletionsOptions()
{
    Messages =
    {
        new ChatMessage(ChatRole.System, """
            You are a salesperson in a flower shop. You must support customers in 
            deciding which bouquet or bouquets he or she wants. If the customer 
            doesn't know which flowers he or she wants, help by asking for what 
            they are buying the flowers, ask for things like their favorite color, 
            and then make suggestions.

            In your shop, you offer the following flowers:

            * Rose (red, yellow, purple)
            * Lily (yellow, pink, white)
            * Gerbera (pink, red, yellow)
            * Freesia (white, pink, red, yellow)
            * Tulips (red, yellow, purple)
            * Sunflowers (yellow)

            Your pricing schema:

            * Small bouquet for 15€ (3 flowers arranged with a little bit of green grass)
            * Medium bouquet for 25€ (5 flowers nicely arranged, including some larger green leaves as decoration)
            * Large bouquet for 35€ (10 flowers, beautifully arranged with greenery and smaller filler flowers)

            Start the conversation by greeting the customer. Welcome them to our 
            shop and mention our slogan "let flowers draw a smile on your face". 
            Ask them what they want. Wait for their response. Based on their response, 
            ask further questions until you know what they want to order.

            Avoid enumerations, be friendly, and avoid being overly excited.

            If the customer asks anything unrelated to flowers and bouquets, tell the 
            customer that you can only respond to flower-related questions.
            """),
        new ChatMessage(ChatRole.User, "Hello!"),
    },
    Functions =
    {
            AddBouquetFunction
    }
};

var oai = new OpenAIClient(new Uri(config["AZURE_ENDPOINT"]!), new AzureKeyCredential(config["AZURE_API_KEY"]!));

while (true)
{
    Console.Write("💐> ");
    var userMessage = Console.ReadLine();
    completionOptions.Messages.Add(new(ChatRole.User, userMessage));

    bool repeat;
    do
    {
        repeat = false;

        var response = await oai.GetChatCompletionsAsync(config["AZURE_MODEL"]!, completionOptions);

        var assistantMessage = response.Value!.Choices[0].Message;
        if (assistantMessage.Content != null)
        {
            Console.WriteLine($"\n🤖> {assistantMessage.Content}\n");
            completionOptions.Messages.Add(assistantMessage);
        }
        else if (assistantMessage.FunctionCall?.Name != null)
        {
            // PROBLEM: Sometimes ChatGPT haluzinates functions!!

            Console.WriteLine($"\n🤖 Calling {assistantMessage.FunctionCall.Name}");

            var bouquet = JsonSerializer.Deserialize<Bouquet>(assistantMessage.FunctionCall.Arguments, jsonOptions);
            // PROBLEM: Sometimes ChatGPT sends INVALID JSON!!

            orderedBouquets.Add(bouquet);

            Console.WriteLine($"\t{JsonSerializer.Serialize(bouquet, jsonOptions)}\n");
            completionOptions.Messages.Add(assistantMessage);
            completionOptions.Messages.Add(new()
            {
                Role = ChatRole.Function,
                Name = assistantMessage.FunctionCall.Name,
                Content = JsonSerializer.Serialize(new FunctionResult(
                    JsonSerializer.Serialize(orderedBouquets, jsonOptions)
                ), jsonOptions)
            });
            repeat = true;
        }
    }
    while (repeat);
}

record Flower(string FlowerType, int Amount);
record Bouquet(string BouquetSize, decimal Price, Flower[] Flowers);
record FunctionResult(string Result);
