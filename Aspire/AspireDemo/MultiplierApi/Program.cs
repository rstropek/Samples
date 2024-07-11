var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
var app = builder.Build();

app.MapDefaultEndpoints();
app.UseHttpsRedirection();

app.MapPost("/multiply", (InputDto input) => new OutputDto(input.ValueToMulitply * 2));

app.Run();

record InputDto(int ValueToMulitply);
record OutputDto(int MultipliedValue);
