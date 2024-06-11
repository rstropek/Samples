// In this sample, we are going to build a web API using Native AOT. We also assume
// that the code will run behind a reverse proxy that will handle TLS termination.
// Therefore, we use the new SlimBuilder: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/native-aot#the-createslimbuilder-method

var builder = WebApplication.CreateSlimBuilder(args);

var section = builder.Configuration.GetSection("GroupLimits");
builder.Services.Configure<GroupLimits>(section);
// Add the options validator to the DI container.
builder.Services.AddSingleton<IValidateOptions<GroupLimits>, GroupLimitsValidator>();

section = builder.Configuration.GetSection("AppFeatures");
// Note that the following call will be intercepted by the Configuration-binding
// source generator. Check the generated code in the obj folder to learn more.
// Read more at https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#configuration-binding-source-generator
var opt = section.Get<AppFeatures>() ?? throw new InvalidOperationException($"Missing configuration section {section.Path}");

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
    IServiceProvider serviceProvider, 
    IOptions<GroupLimits> limits,
    ILogger<Program> logger,
    // Note the use of AsParameters to bind request body, route parameters, and query string parameters.
    [AsParameters] GetQuoteRequestDto payload) =>
{
    // Ensure that a ticket info is present
    if (payload.TicketInfo is null)
    {
        return Results.BadRequest("TicketInfo is required.");
    }

    // Note how we use the keyed service to get the right pricing calculator.
    var pricingCalculator = serviceProvider.GetKeyedService<IPriceCalculator>(payload.PricingScheme);
    if (pricingCalculator is null)
    {
        return Results.BadRequest($"Pricing scheme '{payload.PricingScheme}' is unkonwn");
    }

    // Ensure that the group size is within the limits. Limits are configured
    // via the options pattern.
    var maxGroupSize = payload.PricingScheme switch 
    {
        IPriceCalculator.RegularPricing => limits.Value.MaximumGroupSizeRegular,
        IPriceCalculator.SchoolPricing => limits.Value.MaximumGroupSizeSchool,
        _ => throw new InvalidOperationException($"Pricing scheme '{payload.PricingScheme}' is unkonwn, this should NEVER happen.")
    };
    if (payload.TicketInfo.NumberOfParticipants > maxGroupSize)
    {
        Log.InvalidGroupSize(logger, payload.TicketInfo.NumberOfParticipants, payload.PricingScheme, maxGroupSize);
        return Results.BadRequest($"Maximum group size for pricing scheme '{payload.PricingScheme}' is {maxGroupSize}.");
    }

    // All parameters are fine, so we can calculate the price.
    var price = pricingCalculator.CalculateCoursePrice(
        payload.TicketInfo.BasePrice, 
        payload.TicketInfo.NumberOfParticipants, 
        (payload.OffPeak ?? false) ? CourseTime.OffPeek : CourseTime.Peak);
    return Results.Ok(new GetQuoteResponseDto(price));
});

app.Run();

record TicketInfoDto(decimal BasePrice, int NumberOfParticipants);
record GetQuoteRequestDto(
    string PricingScheme, // Will come from route parameter.
    TicketInfoDto? TicketInfo, // Will come from request body.
    bool? OffPeak); // Will (optionally) come from query string parameter.
record GetQuoteResponseDto(decimal Price);

// Generate type info for JSON serialization at compile time using source generator.
[JsonSerializable(typeof(GetQuoteRequestDto))]
[JsonSerializable(typeof(GetQuoteResponseDto))]
internal partial class DtoJsonSerializerContext : JsonSerializerContext { }
