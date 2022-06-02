using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.MapPost("/calculate", (CalculateDto input) => 
{
    if (input.ValueIncl.HasValue == input.ValueExcl.HasValue)
    {
        return Results.BadRequest(new ProblemDetails
        {
            Type = "https://example.com/invalid-values",
            Title = "Invalid values",
            Detail = "Please provide either a valueIncl or valueExcl"
        });
    }

    if (!input.VatPercentage.HasValue)
    {
        return Results.BadRequest(new ProblemDetails
        {
            Type = "https://example.com/missing-vat-percentage",
            Title = "Missing VAT percentage",
            Detail = "Please provide a VAT percentage"
        });
    }

    if (input.VatPercentage is < 0m or > 1m)
    {
        return Results.BadRequest(new ProblemDetails
        {
            Type = "https://example.com/invalid-vat-percentage",
            Title = "Invalid VAT percentage",
            Detail = "Please provide a VAT percentage between 0.00 (=0%) and 1.00 (=100%)"
        });
    }

    var output = new CalculateDto(0m, input.VatPercentage.Value, 0m);
    if (input.ValueIncl.HasValue) {
        output = output with
        { 
            ValueIncl = input.ValueIncl,
            ValueExcl = input.ValueIncl / (1 + input.VatPercentage)
        };
    } else {
        output = output with
        { 
            ValueExcl = input.ValueExcl,
            ValueIncl = input.ValueExcl * (1 + input.VatPercentage)
        };
    }

    return Results.Ok(output);
});
app.Run();

public partial class Program { }

public record CalculateDto(decimal? ValueIncl, decimal? VatPercentage, decimal? ValueExcl);
