using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<WeatherApi>();

var app = builder.Build();
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    await next();
});
app.MapGet("/div", (int? x, int? y, ILogger<Program> logger) =>
{
    logger.LogInformation("Performing division {x}/{y}", x, y);

    x ??= 42;
    y ??= 0;
    try
    {
        return Results.Ok((x / y).ToString());
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Exception while performing division {x}/{y}", x, y);
        throw;
    }
});
app.MapGet("weather", async (string city, WeatherApi weatherApi, ILogger<Program> logger) =>
{
    if (string.IsNullOrEmpty(city))
    {
        return Results.BadRequest();
    }

    try
    {
        return Results.Ok(await weatherApi.GetCurrentWeather(city));
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Exception while querying weather for {city}", city);
        throw;
    }
});

app.Run();

