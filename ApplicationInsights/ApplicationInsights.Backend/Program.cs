var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();
app.UseHttpsRedirection();
app.Use(async (context, next) =>
{
    await next();
});
app.MapGet("/div", (int? x, int? y) =>
{
    x ??= 42;
    y ??= 0;
    return Results.Ok((x / y).ToString());
});

app.Run();

