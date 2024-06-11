using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder().AddUserSecrets<Program>();
var config = builder.Build();

const string AssistantName = "Fundraising Assistant";
//const string Model = "gpt-4-1106-preview";
const string Model = "gpt-3.5-turbo-1106";

#region Setting up OpenAI API client
var httpClient = new HttpClient()
{
    BaseAddress = new Uri("https://api.openai.com/")
};
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", config["OAIKEY"]);
httpClient.DefaultRequestHeaders.Add("OpenAI-Beta", "assistants=v1");
httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
#endregion

#region Get a list of existing assistants
// Check if assistant already exists. Note that this request has a default limit
// of 20 items per page. To keep things simple, we do not implement paging here and
// assume that there are less than 20 assistants.
System.Console.WriteLine("Getting list of existing assistants...");
var assistents = await httpClient.GetFromJsonAsync<OaiResult<Assistant>>("v1/assistants");
Debug.Assert(assistents != null);

// Look for our assistant in the list of existing assistants
var existingAssistant = assistents.Data.FirstOrDefault(a => a.Name == AssistantName);
#endregion

#region Define masterdata for our assistant
var assistant = new Assistant(
    AssistantName,
    "Assistant used in a fundraising scenario helping fundraisers to store visits",
    Model,
    """
    You are a helpful assistant supporting people doing fundraising by visiting 
    people in their community. Fundraisers will tell you about which households
    they visited (town name, street name, house number, family name). Additionally,
    they will tell you whether they met someone or not. Fundraising happens in Austria, 
    so town and street names are in German.

    Try to identify the necessary data about the household and the flag whether someone
    was met or not. Ask the fundraiser questions until you have all the necessary data.
    Once you have the data, call the function 'store_visit' with the data as parameters.
    """,
    [
        new (
            new(
                "store_visit",
                "Stores a visit in the database",
                new(
                    Properties: new()
                    {
                        ["townName"] = new("string", "Name of the town of the visited household"),
                        ["streetName"] = new("string", "Name of the street of the visited household"),
                        ["houseNumber"] = new("string", "House number of the visited household"),
                        ["familyName"] = new("string", "Family name of the visited household"),
                        ["successfullyVisited"] = new("boolean", "Value indicating whether someone was met or not"),
                    },
                    Required: ["townName", "streetName", "houseNumber", "familyName", "successfullyVisited"])
            )
        )
    ]
);
#endregion

#region Update or create assistant
HttpResponseMessage? response = null;
if (existingAssistant != null)
{
    if (assistant.Name != existingAssistant.Name ||
        assistant.Description != existingAssistant.Description ||
        assistant.Model != existingAssistant.Model ||
        assistant.Instructions != existingAssistant.Instructions ||
        JsonSerializer.Serialize(assistant.Tools) != JsonSerializer.Serialize(existingAssistant.Tools))
    {
        Console.WriteLine("Updating existing assistant...");
        response = await httpClient.PostAsJsonAsync($"v1/assistants/{existingAssistant.Id}", assistant);
    }
    else
    {
        Console.WriteLine("Assistant already up-to-date.");
    }
}
else
{
    Console.WriteLine("Creating assistant...");
    response = await httpClient.PostAsJsonAsync("v1/assistants", assistant);
}

if (response != null)
{
    response?.EnsureSuccessStatusCode();
    existingAssistant = await response?.Content.ReadFromJsonAsync<Assistant>()!;
    Debug.Assert(existingAssistant != null);
    Debug.Assert(existingAssistant.Id != null);
}
#endregion

#region Create thread
Console.WriteLine("Creating thread...");
var newThreadResponse = await httpClient.PostAsync("v1/threads", new StringContent("", Encoding.UTF8, "application/json"));
Debug.Assert(newThreadResponse != null);
newThreadResponse.EnsureSuccessStatusCode();
var newThread = await newThreadResponse.Content.ReadFromJsonAsync<CreateThreadResult>();
Debug.Assert(newThread != null);
var threadId = newThread.Id;
#endregion

#region Add message
Console.WriteLine("Adding message...");
var newMessageResponse = await httpClient.PostAsJsonAsync($"v1/threads/{threadId}/messages", new CreateThreadMessage(
    """
    I just was at Birkenweg 16 in Leonding visiting family Maier. They were at home.
    """
    // """
    // I just visited someone
    // """
    ));
Debug.Assert(newMessageResponse != null);
newMessageResponse.EnsureSuccessStatusCode();
#endregion

#region Create Run
Console.WriteLine("Creating run...");
var newRunResponse = await httpClient.PostAsJsonAsync($"v1/threads/{threadId}/runs", new CreateRun(existingAssistant!.Id!));
Debug.Assert(newRunResponse != null);
newRunResponse.EnsureSuccessStatusCode();
var newRun = await newRunResponse.Content.ReadFromJsonAsync<Run>();
Debug.Assert(newRun != null);
#endregion

#region Wait for completed
var loop = false;
do
{
    Console.WriteLine("Waiting for run to complete...");
    var max = 10;
    while (newRun.Status is not "completed" and not "requires_action" && max >= 0)
    {
        Console.WriteLine("\tChecking run status...");
        await Task.Delay(1000);
        max--;
        var runResponse = await httpClient.GetAsync($"v1/threads/{threadId}/runs/{newRun.Id}");
        Debug.Assert(runResponse != null);
        runResponse.EnsureSuccessStatusCode();
        newRun = await runResponse.Content.ReadFromJsonAsync<Run>();
        Debug.Assert(newRun != null);
        System.Console.WriteLine($"\tRun status: {newRun.Status}");
    }

    switch (newRun.Status)
    {
        case "completed":
            {
                Console.WriteLine("\tListing messages of thread...");
                var messages = await httpClient.GetFromJsonAsync<OaiResult<Message>>($"v1/threads/{threadId}/messages");
                Debug.Assert(messages != null);
                foreach (var m in messages.Data)
                {
                    foreach (var c in m.Content)
                    {
                        Console.WriteLine($"\t\t{m.Role}: {c.Text.Value}");
                    }
                }

                break;
            }
        case "requires_action":
            {
                Console.WriteLine("\tRun requires action. Retrieving run details...");
                var run = await httpClient.GetFromJsonAsync<Run>($"v1/threads/{threadId}/runs/{newRun.Id}");
                Debug.Assert(run != null);
                var functionName = run.RequiredAction.SubmitToolOutputs?.ToolCalls[0].Function.Name;
                var arguments = run.RequiredAction.SubmitToolOutputs?.ToolCalls[0].Function.Arguments;
                Debug.Assert(functionName != null);
                Debug.Assert(arguments != null);
                Console.WriteLine($"\tFunction '{functionName}' called with arguments '{arguments}'");

                var visitArguments = JsonSerializer.Deserialize<VisitArguments>(arguments);

                System.Console.WriteLine("Submit tool output to run...");
                var toolOutputResponse = await httpClient.PostAsJsonAsync($"v1/threads/{threadId}/runs/{newRun.Id}/submit_tool_outputs", new ToolsOutput(
                    run.RequiredAction.SubmitToolOutputs!.ToolCalls[0].Id,
                    $"{{\"visitId\": \"1234\"}}"
                ));
                Debug.Assert(toolOutputResponse != null);
                toolOutputResponse.EnsureSuccessStatusCode();

                loop = true;

                break;
            }
    }
}
while (loop);
#endregion

// Console.Write("Your message: ");
// var message = Console.ReadLine()!;

#region Delete thread
Console.WriteLine("Deleting thread...");
var deleteThreadResponse = await httpClient.DeleteAsync($"v1/threads/{threadId}");
Debug.Assert(deleteThreadResponse != null);
deleteThreadResponse.EnsureSuccessStatusCode();
#endregion

#region DTOs for OpenAI
// Note that normally, we would use Microsoft's Nuget package for OpenAI access (https://www.nuget.org/packages/Azure.AI.OpenAI). 
// However, the current version does not support the Beta APIs from OpenAI. Thererfore, we have to implement
// the DTOs ourselves. You can track the progress of the new OpenAI features in the Azure.AI.OpenAI package here:
// https://github.com/Azure/azure-sdk-for-net/issues/40347

record OaiResult<T>(
    T[] Data
);

record Assistant(
    string Name,
    string Description,
    string Model,
    string Instructions,
    FunctionToolEnvelope[] Tools)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Id { get; set; }
}

record FunctionToolEnvelope(
    FunctionTool Function)
{
    public string Type => "function";
}

record FunctionTool(
    string Name,
    string Description,
    FunctionParameters Parameters
);

record FunctionParameters(
    Dictionary<string, FunctionParameter> Properties,
    string[] Required
)
{
    public string Type => "object";
}

record FunctionParameter(
    string Type,
    string Description
);

record CreateThread();

record CreateThreadResult(
    string Id
);

record CreateThreadMessage(
    string Content
)
{
    public string Role => "user";
}

record Message(
    string Id,
    MessageContent[] Content,
    [property: JsonPropertyName("thread_id")] string ThreadId,
    string Role,
    [property: JsonPropertyName("assistant_id")] string AssistantId,
    [property: JsonPropertyName("run_id")] string RunId
);

record MessageContent(
    MessageContentText Text
)
{
    public string Type => "text";
}

record MessageContentText(
    string Value
);

record CreateRun(
    [property: JsonPropertyName("assistant_id")] string AssistantId
);

record Run(
    string Id,
    [property: JsonPropertyName("thread_id")] string ThreadId,
    [property: JsonPropertyName("assistant_id")] string AssistantId,
    string Status,
    [property: JsonPropertyName("required_action")] RequiredAction RequiredAction,
    [property: JsonPropertyName("last_error")] string LastError
);

record RequiredAction(
    string Type,
    [property: JsonPropertyName("submit_tool_outputs")] SubmitToolOutputs? SubmitToolOutputs
);

record SubmitToolOutputs(
    [property: JsonPropertyName("tool_calls")] ToolCall[] ToolCalls
);

record ToolCall(
    string Id,
    FunctionToolCall Function
);

record FunctionToolCall(
    string Name,
    string Arguments
);

record VisitArguments(
    string TownName,
    string StreetName,
    string HouseNumber,
    string FamilyName,
    bool SuccessfullyVisited
);

record ToolsOutput(
    [property: JsonPropertyName("tool_call_id")] string ToolCallId,
    string Output
);

#endregion
