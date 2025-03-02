using MediatR;
using Registration.Api;
using Registration.CreateCampaign;

var builder = WebApplication.CreateBuilder(args);
builder.AddJsonFileRepository();
builder.Services.AddMediatR();
builder.Services.AddExceptionHandler();
var app = builder.Build();

app.UseStatusCodePages();
app.UseExceptionHandler();

app.MapGet("/ping", () => "pong");

app.MapPost("/campaigns", async (CreateCampaignRequest request, IMediator mediator) =>
{
    var result = await mediator.Send(new CreateCampaign(request));
    return Results.Ok(result);
});

app.Run();
