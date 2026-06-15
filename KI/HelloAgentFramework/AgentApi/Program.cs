using System.ComponentModel;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

// Aspire service defaults: OpenTelemetry, health checks, service discovery, resilience.
builder.AddServiceDefaults();

// Dev-only CORS so the cross-origin Vite client can POST to and read the SSE stream from
// the AG-UI endpoint. AllowAnyOrigin is fine here because the agent is unauthenticated and
// this sample is meant for local testing only.
const string CorsPolicy = "frontend";
builder.Services.AddCors(options => options.AddPolicy(
    CorsPolicy,
    policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// The chat client points at a *local* Ollama instance. Endpoint and model come from
// configuration (appsettings.json), with sensible defaults; the AppHost overrides them
// via environment variables (Ollama__Endpoint / Ollama__Model).
var ollamaEndpoint = builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434";
var ollamaModel = builder.Configuration["Ollama:Model"] ?? "gpt-oss:20b";

// OllamaApiClient implements Microsoft.Extensions.AI.IChatClient, so it plugs straight
// into the Agent Framework.
IChatClient chatClient = new OllamaApiClient(new Uri(ollamaEndpoint), ollamaModel);

// Build the agent from the chat client. AsAIAgent wires automatic function (tool)
// invocation, so the model can call AddNumbers on its own and the result is fed back
// before the final answer is streamed.
AIAgent agent = chatClient.AsAIAgent(
    instructions: "You are a helpful assistant.",
    name: "HelloAgent",
    // An explicit tool name keeps the AG-UI TOOL_CALL events readable (otherwise the
    // local function shows up under its compiler-generated name).
    tools: [AIFunctionFactory.Create(AddNumbers, name: "AddNumbers")]);

var app = builder.Build();

app.UseCors(CorsPolicy);
app.MapDefaultEndpoints();

// Expose the agent over the AG-UI protocol (HTTP POST + Server-Sent Events) at /ag-ui.
// The hand-written browser client talks to this endpoint directly.
app.MapAGUI("/ag-ui", agent);

app.Run();

// A single demo tool. The [Description] attributes become the tool/parameter schema that
// is advertised to the model.
[Description("Adds two numbers and returns their sum.")]
static double AddNumbers(
    [Description("The first number")] double a,
    [Description("The second number")] double b)
    => a + b;
