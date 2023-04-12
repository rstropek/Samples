#region Configure and create application builder
// The minimal API in ASP.NET Core is built using the BUILDER PATTERN, which is a design 
// pattern that provides a flexible and modular way to construct complex objects.
// Here we see how we use the builder pattern with ASP.NET Core. ImageOptionsBuilder.cs
// contains a sample for how to create your own builder classes/interfaces.

// We start by adding services to the service collection. The service collection is a
// collection of services that are used by the application. Services are objects that
// provide functionality to the application. For example, the service collection contains
// services for logging, dependency injection, configuration, routing, and so on.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddEndpointsApiExplorer()      // Use extension method to add API explorer services
    .AddSwaggerXmlDocumentation()   // Use custom extension method to add C# XML documentation to Swagger
    .AddCorsAllowAll()              // Use custom extension method to add CORS support (allow all, for demo purposes only)
    .ConfigureJsonOptions()         // Use custom extension method to configure JSON serialization options
    .AddDataProtection();           // Use extension method to add data protection services

// Use extension methods to add additional application services. When the list of
// services becomes too long, it is recommended to move the service registration to
// a separate method/class (similar to the custom extension methods above).
builder.Services
    .AddSingleton<IImageOptionsProtector, ImageOptionsProtector>()
    .AddTransient<IImageOptionBuilder, ImageOptionBuilder>()
    .AddSingleton<IImageComponentCache>((_) => new ImageComponentCache().Fill())
    .AddSingleton<IImageRenderer, ImageRenderer>()
    .AddScoped<IValidator<ImageOptions>, ImageOptionsValidator>();

// Finally, once all services have been added, we can construct the application builder.
// The application builder is used to configure the application pipeline, which is a sequence 
// of middleware components that handle requests and responses in the application.
var app = builder.Build();
#endregion

#region Add supporting middleware to application pipeline
app.UseCors()
    .UseSwagger()
    .UseSwaggerUI()
    .HandleException<InvalidImageOptionsException>(HttpStatusCode.BadRequest);
#endregion

#region API endpoint: Build image URL from image options
app.MapPost("/build-image-url", (ImageOptions options, HttpRequest request, IImageOptionsProtector protector) =>
{
    // No need to validate options here, because we have an endpoint filter.
    // Read more: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/min-api-filters

    var protectedString = protector.Protect(options);

    // Build image URL
    var url = $"{request.Scheme}://{request.Host}/img/{protectedString}";
    return Results.Created(url, new BuildImageUrlResponse(url, protectedString));
})
.AddEndpointFilter<Validator<ImageOptions>>()
.WithName("BuildImageUrl")
.WithSummary("""
    Builds an image URL for the given image options. You can use the resulting URL e.g. in a HTML img tag.
    Note that the generated image URL is only valid for 1 minute!
    """)
.Produces<BuildImageUrlResponse>(StatusCodes.Status201Created)
.Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
.WithOpenApi(o =>
{
    o.Responses[StatusCodes.Status201Created.ToString()].Description = "Successfully built image URL";
    o.Responses[StatusCodes.Status201Created.ToString()].Headers.Add("Location", new() { Description = "The URL of the image", Schema = new() { Type = "string" } });
    o.Responses[StatusCodes.Status400BadRequest.ToString()].Description = "Invalid image options, see body for details.";
    return o;
});
#endregion

#region API endpoint: Build image URLs with builder
app.MapGet("/images/happy", (IImageOptionBuilder builder) 
    => Results.Ok(builder
        .Happy()
        .WithHammer()
        .Build()))
.WithName("GetHappyImageOptions")
.WithSummary("""
    Generates image options for a happy image. You can use the generated image options to
    build an image URL using the /build-image-url API endpoint.
    """)
.Produces<ImageOptions>(StatusCodes.Status200OK)
.WithOpenApi();

app.MapGet("/images/sad", (IImageOptionBuilder builder) 
    => Results.Ok(builder
        .WithEye(EyeType.HalfOpen)
        .WithMouth(MouthType.Unhappy)
        .WithRightHand(RightHandType.Normal).Build()))
.WithName("GetSadImageOptions")
.WithSummary("""
    Generates image options for a sad image. You can use the generated image options to
    build an image URL using the /build-image-url API endpoint.
    """)
.Produces<ImageOptions>(StatusCodes.Status200OK)
.WithOpenApi();

app.MapGet("/images/random", (IImageOptionBuilder builder) 
    => Results.Ok(builder
    .Random()
    .Build()))
.WithName("GetRandomImageOptions")
.WithSummary("""
    Generates random image options. You can use the generated image options to
    build an image URL using the /build-image-url API endpoint.
    """)
.Produces<ImageOptions>(StatusCodes.Status200OK)
.WithOpenApi();
#endregion

#region API endpoint: Render image
app.MapGet("/img/{imageId}", (string imageId, float? scale /* = 1f */, IImageOptionsProtector protector, IImageRenderer renderer) =>
{
    // Decode and decrypt the image options
    var imageOptions = protector.Unprotect(imageId);

    // Note that we need to handle default value in code. This is because the default values
    // will only be supported in the upcoming C# version. Read more at
    // https://github.com/dotnet/csharplang/issues/6051
    scale ??= 1f;

    // Render image and return it
    var data = renderer.Render(imageOptions, scale.Value);
    return Results.File(data, "image/png");
})
.AddEndpointFilter(async (efiContext, next) =>
{
    var scale = efiContext.Arguments.OfType<float?>().FirstOrDefault();
    if (scale != null && (scale < 0.1f || scale > 2f))
    {
        throw new InvalidImageOptionsException($"Invalid scale {scale}, must be between 0.1 and 2");
    }

    return await next(efiContext);
})
.WithName("GetImage")
.WithSummary("""
    Returns the image. Use this endpoint e.g. in a HTML img tag. Note that image URLs created with 
    the /build-image-url API endpoint are only valid for 1 minute!
    """)
.Produces(StatusCodes.Status200OK)
.Produces<string>(StatusCodes.Status400BadRequest)
.WithOpenApi(o =>
{
    o.Responses[((int)StatusCodes.Status200OK).ToString()].Description = "Successfully built image.";
    o.Responses[((int)StatusCodes.Status400BadRequest).ToString()].Description = "Invalid image options, see body for details.";
    o.Parameters[0].Description = """
        Image ID that must have been generated with the */build-image-url* API endpoint.
        Note that image IDs created with the */build-image-url* API endpoint are **only valid for 1 minute**!
        """;
    o.Parameters[1].Description = "Image scaling factor, must be between 0.1 and 2. Defaults to 1.";
    return o;
});
#endregion

app.Run();

record BuildImageUrlResponse(string Url, string ImageId)
{
    /// <summary>
    /// The URL to the generated image. Note that this URL is only valid for 1 minute!
    /// </summary>
    public string Url { get; init; } = Url;
}

class Validator<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext efiContext, EndpointFilterDelegate next)
    {
        if (efiContext.Arguments.Any(a => a is T))
        {
            var validator = efiContext.HttpContext.RequestServices.GetRequiredService<IValidator<T>>();
            foreach (var arg in efiContext.Arguments.Where(a => a is T))
            {
                var validationResult = validator.Validate((T)arg!);
                if (!validationResult.IsValid)
                {
                    throw new InvalidImageOptionsException(validationResult.Errors);
                }
            }
        }

        return await next(efiContext);
    }
}
