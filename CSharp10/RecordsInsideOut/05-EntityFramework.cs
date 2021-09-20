namespace RecordsInsideOut;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class EntityFramework
{
    private record Hero(int ID = 0, string Name = "", string Universe = "", bool CanFly = false);

    class HeroContext : DbContext
    {
        public HeroContext(DbContextOptions<HeroContext> options) : base(options) { }

        // A record is just a special form of a class, so we
        // can use it with Entity Framework.
        public DbSet<Hero> Heroes => Set<Hero>();
    }

    private class HeroContextFactory : IDesignTimeDbContextFactory<HeroContext>
    {
        public HeroContext CreateDbContext(string[] args) =>
            new(new DbContextOptionsBuilder<HeroContext>()
                .UseInMemoryDatabase("Heroes")
                .Options);
    }

    [Fact]
    public async Task Records_With_EFCore()
    {
        var factory = new HeroContextFactory();
        using var context = factory.CreateDbContext(Array.Empty<string>());

        // Add a record to the database.
        var hero = new Hero(0, "Homelander", "DC", true);
        context.Heroes.Add(hero);
        await context.SaveChangesAsync();

        // Note that the ID will be set by EFCore, but NOT
        // using the clone/`with` mechanism. The ID is changed
        // in the existing object -> breaks immutability rule.
        Assert.NotEqual(0, hero.ID);

        // If we want to update an object, we have to detach
        // it first because we must create a copy of the record
        // and attach the copy instead of the original object.
        context.Entry(hero).State = EntityState.Detached;
        hero = hero with { Name = "Stormfront" };
        context.Heroes.Update(hero);
        await context.SaveChangesAsync();

        // Querying with records works just fine.
        var heroes = await context.Heroes.ToArrayAsync();
        Assert.Single(heroes);
        Assert.Equal("Stormfront", heroes.First().Name);
    }
}
