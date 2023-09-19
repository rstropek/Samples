// In this sample, we are going to build a web API using Native AOT. We also assume
// that the code will run behind a reverse proxy that will handle TLS termination.
// Therefore, we use the new SlimBuilder: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/native-aot#the-createslimbuilder-method

var builder = WebApplication.CreateSlimBuilder(args);

var section = builder.Configuration.GetSection("AppFeatures");
// Note that the following line does not work in RC 1. Seems to be a bug.
// Read more at https://github.com/dotnet/runtime/issues/92273
// builder.Services.Configure<AppFeatures>(section);

// Note that the following call will be intercepted by the Configuration-binding
// source generator. Check the generated code in the obj folder to learn more.
// Read more at https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#configuration-binding-source-generator
var opt = section.Get<AppFeatures>();
Debug.Assert(opt != null);

// Note that OpenAPI aka Swagger is not supported in Native AOT yet.
// Read more at https://github.com/dotnet/aspnetcore/issues/45910

builder.Services.ConfigureJsonOptions();

// Here we use the new keyed DI services to add two flavors of the pricing calculator.
// Read more at https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#keyed-di-services
builder.Services
    .AddKeyedSingleton<IPriceCalculator, RegularPricing>(IPriceCalculator.RegularPricing)
    .AddKeyedSingleton<IPriceCalculator, SchoolPricing>(IPriceCalculator.SchoolPricing);

var app = builder.Build();

var api = app.MapGroup("/api");
if (opt.ProvideMasterdata)
{
    api.MapGet("/locations", () => MasterData.Locations);
}

api.MapPost("/quote/{pricingScheme}", (
    IServiceProvider ServiceProvider, 
    // Note the use of AsParameters to bind request body, route parameters, and query string parameters.
    [AsParameters] GetQuoteRequestDto payload) =>
{
    if (payload.TicketInfo is null)
    {
        return Results.BadRequest("TicketInfo is required.");
    }

    // Note how we use the keyed service to get the right pricing calculator.
    var pricingCalculator = ServiceProvider.GetKeyedService<IPriceCalculator>(payload.PricingScheme);
    if (pricingCalculator is null)
    {
        return Results.BadRequest($"Pricing scheme '{payload.PricingScheme}' is unkonwn");
    }

    var price = pricingCalculator.CalculateCoursePrice(
        payload.TicketInfo.BasePrice, 
        payload.TicketInfo.NumberOfParticipants, 
        payload.OffPeak ? CourseTime.OffPeek : CourseTime.Peak);
    return Results.Ok(new GetQuoteResponseDto(price));
});

app.Run();

record TicketInfoDto(decimal BasePrice, int NumberOfParticipants);
record GetQuoteRequestDto(
    string PricingScheme, // Will come from route parameter.
    TicketInfoDto? TicketInfo, // Will come from request body.
    bool OffPeak); // Will come from query string parameter.
record GetQuoteResponseDto(decimal Price);

// Generate type info for JSON serialization at compile time using source generator.
[JsonSerializable(typeof(GetQuoteRequestDto))]
[JsonSerializable(typeof(GetQuoteResponseDto))]
internal partial class DtoJsonSerializerContext : JsonSerializerContext { }
