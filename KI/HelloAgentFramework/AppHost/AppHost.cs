var builder = DistributedApplication.CreateBuilder(args);

// The Agent Framework service that exposes the AG-UI endpoint. It talks to the local
// Ollama instance; endpoint and model are passed as environment overrides (they also
// have defaults baked into AgentApi's appsettings.json).
var agent = builder.AddProject<Projects.AgentApi>("agentapi")
    .WithEnvironment("Ollama__Endpoint", "http://localhost:11434")
    .WithEnvironment("Ollama__Model", "gpt-oss:20b");

// The hand-written Vite + React client. It calls the agent cross-origin (CORS is enabled
// on the agent), so it only needs to know the agent's URL. We expose that URL to the
// browser through a VITE_-prefixed env var, which Vite surfaces as import.meta.env.
builder.AddViteApp("frontend", "../Frontend")
    .WithReference(agent)
    .WaitFor(agent)
    .WithEnvironment("VITE_AGENT_URL", agent.GetEndpoint("http"))
    .WithNpmPackageInstallation();

builder.Build().Run();
