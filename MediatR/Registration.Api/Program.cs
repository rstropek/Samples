using DataAccess;
using FluentValidation;
using MediatR;
using Registration;
using Registration.Api;
using Registration.CreateCampaign;

var builder = WebApplication.CreateBuilder(args);

// Create the data directory if it does not exist.
var section = builder.Configuration.GetSection("RepositorySettings");
var repositorySettings = section.Get<RepositorySettings>()
    ?? throw new InvalidOperationException("RepositorySettings are not configured");
var dataPath = repositorySettings.DataFolder;
if (!Directory.Exists(dataPath)) { Directory.CreateDirectory(dataPath); }

builder.Services.AddSingleton<IJsonFileRepository>(_ => new JsonFileRepository(repositorySettings));
builder.Services.AddValidatorsFromAssemblyContaining<CreateCampaign>();
builder.Services.AddMediatR(cfg =>
{
    cfg.AddOpenRequestPreProcessor(typeof(ValidationBehavior<>));
    cfg.RegisterServicesFromAssemblyContaining<CreateCampaign>();
});
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("trace-id", ctx.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<ExceptionToProblemDetailsHandler>();
var app = builder.Build();

app.UseStatusCodePages();
app.UseExceptionHandler();
app.MapPost("/campaigns", async (CreateCampaignRequest request, IMediator mediator) =>
{
    var result = await mediator.Send(new CreateCampaign(request));
    return Results.Ok(result);
});

app.Run();
