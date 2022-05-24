using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseHttpsRedirection();

var Heroes = new ConcurrentDictionary<int, Hero>
{
    [1] = new(1, "Homelander", "DC", true),
    [2] = new(2, "Groot", "Marvel", false),
};

app.MapGet("/api/heroes", () => Results.Ok(Heroes));
app.MapGet("/api/heroes/{id}", (int id) => Heroes.TryGetValue(id, out var h) switch
{
    false => Results.NotFound(),
    true => Results.Ok(h),
}).WithName("GetSingleHero");
app.MapPost("/api/heroes", (NewHeroDto h) =>
{
    // Note: This sample does not contain validation in order to keep it simple
    //       and focus on records.

    var newHero = new Hero(0, h.Name, h.Universe, h.CanFly);
    while (true)
    {
        var newId = Heroes.Max(h => h.Key);
        newHero = newHero with { ID = newId };
        if (Heroes.TryAdd(newId, newHero))
        {
            break;
        }
    };

    Results.CreatedAtRoute("GetSingleHero", new { id = newHero.ID }, newHero);
});

app.Run();

record Hero(int ID, string Name, string Universe, bool CanFly);

// Records work very nice as DTOs
record NewHeroDto(string Name, string Universe, bool CanFly);
