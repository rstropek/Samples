using System.Text.Json.Serialization;
using MediatR;
using Registration;
using Registration.Api;

var builder = WebApplication.CreateBuilder(args);
builder.AddJsonFileRepository();
builder.Services.AddMediatR();
builder.Services.AddExceptionHandler();
builder.Services.ConfigureHttpJsonOptions(options => {
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

app.Run();
