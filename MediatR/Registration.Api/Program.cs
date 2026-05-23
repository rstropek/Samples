using System.Text.Json;
using System.Text.Json.Serialization;
using MediatR;
using Registration;
using Registration.Api;

var builder = WebApplication.CreateBuilder(args);
builder.AddJsonFileRepository();
builder.Services.AddMediatR();
builder.Services.AddExceptionHandler();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
var app = builder.Build();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.MapGet("/ping", () => "pong");

app.MapPost("/campaigns", async (CreateCampaignRequest request, IMediator mediator, ResultConverter converter) =>
{
    var result = await mediator.Send(new CreateCampaign(request));
    return converter.ToResult(result);
});

app.MapGet("/campaigns/{id}", async (Guid id, IMediator mediator, ResultConverter converter) =>
{
    var result = await mediator.Send(new GetCampaign(id));
    return converter.ToResult(result);
});

app.MapPatch("/campaigns/{id}", async (Guid id, UpdateCampaignRequest request, IMediator mediator, ResultConverter converter) =>
{
    var result = await mediator.Send(new UpdateCampaign(id, request));
    return converter.ToResult(result);
});

app.MapPost("/campaigns/{id}/activate", async (Guid id, IMediator mediator, ResultConverter converter) =>
{
    var result = await mediator.Send(new ActivateCampaign(id));
    return converter.ToResult(result);
});

app.MapGet("/campaigns", async (IMediator mediator, ResultConverter converter) =>
{
    var result = await mediator.Send(new GetCampaigns());
    return converter.ToResult(result);
});

app.MapGet("campaigns/changes", async (HttpContext ctx, IMediator mediator, CancellationToken cancellationToken) =>
{
    async void OnCampaignChanged(object? sender, CampaignChangedNotification ea)
    {
        var campaigns = await mediator.Send(new GetCampaigns(), cancellationToken);
        if (campaigns.IsFailed) { return; }

        var campaignsToNotify = campaigns.Value.Where(c => c.CampaignId == ea.CampaignId).ToArray();
        if (campaignsToNotify.Length == 0) { return; }

        var msg = JsonSerializer.Serialize(campaignsToNotify);

        await ctx.Response.WriteAsync($"data: ", cancellationToken);
        await ctx.Response.WriteAsync(msg, cancellationToken);
        await ctx.Response.WriteAsync("\n\n", cancellationToken);
        await ctx.Response.Body.FlushAsync(cancellationToken);
    }

    CampaignNotification.CampaignChanged += OnCampaignChanged;
    try
    {
        ctx.Response.Headers.Append("Content-Type", "text/event-stream");
        await ctx.Response.WriteAsync(": stream is starting\n\n", cancellationToken);
        await ctx.Response.Body.FlushAsync(cancellationToken);
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationToken);
        }
    }
    catch (TaskCanceledException)
    {
    }
    finally
    {
        CampaignNotification.CampaignChanged -= OnCampaignChanged;
    }
});

app.Run();
