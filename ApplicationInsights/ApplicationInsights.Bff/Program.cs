using ApplicationInsights.Bff;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.EventCounterCollector;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Reflection;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

var options = new ApplicationInsightsServiceOptions()
{
    // Note: You can specify the application version.
    // Good idea to detect changed runtime behavior after
    // deploying new application versions.
    ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",

    // Add additional configuration values here.
    // See https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#using-applicationinsightsserviceoptions
    // to learn more.

    // Tip: Stay with adaptive sampling if possible for your app.
    // This makes sure, you are not throttled and you get meaningful
    // telemetry even if you app writes a lot of logs.
    // See https://docs.microsoft.com/en-us/azure/azure-monitor/app/sampling#configuring-adaptive-sampling-for-aspnet-core-applications
    // to learn more.
};

// Add a telemetry initializer for enriching request telemetry with user data.
// Read more about telemetry initializers at https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-filtering-sampling#addmodify-properties-itelemetryinitializer.
builder.Services.AddSingleton<ITelemetryInitializer, HttpContextRequestTelemetryInitializer>();

// The following line shows how you could switch from ServerTelemetryChannel
// to InMemoryChannel. Read more about telemetry channels at
// https://docs.microsoft.com/en-us/azure/azure-monitor/app/telemetry-channels.
// builder.Services.AddSingleton(typeof(ITelemetryChannel), new InMemoryChannel() { MaxTelemetryBufferCapacity = 19898 });

// Add AI to web app. Note that AI will read connection string
// from IConfiguration. Therefore, you can configure the connection
// string with any .NET config mechanism (e.g. config file,
// environment variable, Key Vault, etc.).
// Note: Prefer connection string over instrumentation key.
builder.Services.AddApplicationInsightsTelemetry(options);

// Enable collection of EventCounters.
// See https://docs.microsoft.com/en-us/azure/azure-monitor/app/eventcounters#default-counters-collected
// to learn more about EventCounters.
// See https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core#configuring-or-removing-default-telemetrymodules
// to learn how to configure or remove telemetry modules in general.
builder.Services.ConfigureTelemetryModule<EventCounterCollectionModule>((module, o) =>
{
    // This removes all default counters, if any.
    module.Counters.Clear();

    // Add user-defined counters if there are some.
    // See https://docs.microsoft.com/en-us/dotnet/core/diagnostics/event-counters#implement-an-eventsource
    // to learn how to implement an EventSource.

    // This adds the system counter "working-set" from "System.Runtime".
    // See https://docs.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
    // for a list of well-known counters.
    module.Counters.Add(new EventCounterCollectionRequest("System.Runtime", "working-set"));
});

builder.Services.AddCors();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

builder.Services.AddHttpClient("Backend", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["BackendApi"]!, UriKind.Absolute);
});

var app = builder.Build();

// The following code demonstrates how to capture logs
// within ASP.NET Core startup code. Learn more at
// https://docs.microsoft.com/en-us/azure/azure-monitor/app/ilogger#capture-logs-within-aspnet-core-startup-code
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Setting up ASP.NET Pipeline");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler(errorApp =>
{
    // Logs unhandled exceptions. For more information about all the
    // different possibilities for how to handle errors see
    // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling?view=aspnetcore-5.0
    errorApp.Run(async context =>
    {
        // Return machine-readable problem details. See RFC 7807 for details.
        // https://datatracker.ietf.org/doc/html/rfc7807#page-6
        var pd = new ProblemDetails
        {
            Type = "https://demo.api.com/errors/internal-server-error",
            Title = "An unrecoverable error occurred",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "This is a demo error used to demonstrate problem details",
        };
        pd.Extensions.Add("RequestId", context.TraceIdentifier);
        await context.Response.WriteAsJsonAsync(pd, new JsonSerializerOptions(), "application/problem+json", CancellationToken.None);
    });
});
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapGet("/ping", () => Results.Ok("pong"));
app.MapGet("/secret-ping", [Authorize] () => Results.Ok("secret pong"));
app.MapGet("/logger", (ILogger<Program> logger) =>
{
    // We can use the regular ILogger from ASP.NET Core to
    // write logs. Read more at
    // https://docs.microsoft.com/en-us/azure/azure-monitor/app/ilogger.
    // Note that ILogger logs become TraceTelemetry entries in App Insights.
    // Yet, if you log an exception, ExceptionTelemetry entry is created.
    logger.LogWarning("Hmm, I am not sure everything is ok");

    // App Insights also supports ILogger's scope. Scope values will become
    // custom properties in all log entries written inside the scope.
    // See also https://docs.microsoft.com/en-us/azure/azure-monitor/app/ilogger#logging-scopes
    using (logger.BeginScope(new Dictionary<string, object>() { { "Key", "Value" } }))
    {
        logger.LogWarning("Hmm, I am still unsure");
        logger.LogError("Ok, something IS wrong!");
    }

    return Results.StatusCode((int)HttpStatusCode.InternalServerError);
});
app.MapGet("/track-custom-event", (TelemetryClient telemetry) =>
{
    // If we want to write any custom event, metric, request, etc.,
    // we get an instance of TelemetryClient from DI and use the
    // corresponding tracking method. Read more about AI custom events
    // and metrics at https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics
    telemetry.TrackEvent(
        $"This is a custom event", 
        new Dictionary<string, string>() { { "Answer", "Forty two" } }, 
        new Dictionary<string, double>() { { "Runtime", 42d } });

    return Results.Ok();
});
app.MapGet("/div", (int? x, int? y) =>
{
    x ??= 42;
    y ??= 0;
    return Results.Ok((x / y).ToString());
});
app.MapGet("/div-backend", async (int? x, int? y, IHttpClientFactory httpClientFactory, ILogger<Program> logger) =>
{
    x ??= 42;
    y ??= 0;

    var client = httpClientFactory.CreateClient("Backend");
    var response = await client.GetFromJsonAsync<string>($"/div?x={x}&y={y}");
    if (string.IsNullOrEmpty(response))
    {
        logger.LogWarning("Backend responded with empty string");
    }

    return Results.Ok(response);
});
app.MapGet("/weather-backend", async (string city, IHttpClientFactory httpClientFactory, ILogger<Program> logger) =>
{
    var client = httpClientFactory.CreateClient("Backend");
    var response = await client.GetFromJsonAsync<string>($"/weather?city={city}");
    if (string.IsNullOrEmpty(response))
    {
        logger.LogWarning("Backend responded with empty string");
    }

    return Results.Ok(response);
});

logger.LogInformation("Pipeline set up successfully");

app.Run();

