using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder
                .ConfigureServices(services => services.AddControllers())
                .Configure(app =>
                {
                    app.UseRouting();
                    app.UseEndpoints(endpoints => endpoints.MapControllers());
                });
    })
    .Build()
    .Run();

[Route("api/[controller]")]
[ApiController]
public class HeroesController : ControllerBase
{
    public record HeroDto(int ID, string Name, string Universe, bool CanFly);

    private List<HeroDto> Heroes { get; } = new()
    {
        new(1, "Homelander", "DC", true),
        new(2, "Groot", "Marvel", false),
    };

    [HttpGet]
    [Route("")]
    public IActionResult GetAllHeroes() => Ok(Heroes);

    [HttpGet("{id}", Name = nameof(GetHeroById))]
    public IActionResult GetHeroById(int id) =>
        Heroes.FirstOrDefault(h => h.ID == id) switch
        {
            null => NotFound(),
            var h => Ok(h)
        };

    [HttpPost]
    public IActionResult AddHero([FromBody] HeroDto hero)
    {
        Heroes.Add(hero);
        return CreatedAtRoute(nameof(GetHeroById), new { id = hero.ID }, hero);
    }
}
