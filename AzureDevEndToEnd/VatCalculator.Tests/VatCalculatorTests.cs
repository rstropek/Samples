using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace VatCalculator.Tests;

public class VatCalculatorTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> factory;

    public VatCalculatorTests(WebApplicationFactory<Program> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task NoVatPercentage()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/calculate", new CalculateDto(120m, null, null));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("https://example.com/missing-vat-percentage", result!.Type);
    }

    [Fact]
    public async Task InvalidVatPercentage()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/calculate", new CalculateDto(120m, 1.2m, null));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("https://example.com/invalid-vat-percentage", result!.Type);
    }

    [Fact]
    public async Task BothValues()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/calculate", new CalculateDto(120m, 0.2m, 100m));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("https://example.com/invalid-values", result!.Type);
    }

    [Fact]
    public async Task NoValues()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/calculate", new CalculateDto(null, 0.2m, null));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("https://example.com/invalid-values", result!.Type);
    }

    [Fact]
    public async Task FromInclToExcl()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/calculate", new CalculateDto(120m, 0.2m, null));
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CalculateDto>();
        Assert.Equal(100m, result!.ValueExcl);
    }

    [Fact]
    public async Task FromExclToIncl()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/calculate", new CalculateDto(null, 0.2m, 100m));
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<CalculateDto>();
        Assert.Equal(120m, result!.ValueIncl);
    }
}