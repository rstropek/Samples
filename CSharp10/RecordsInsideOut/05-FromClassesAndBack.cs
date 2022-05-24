namespace RecordsInsideOut;
using AutoMapper;

public class FromClassesAndBack
{
    private record Hero(string Name = "", string Universe = "", bool CanFly = false);

    private class TraditionalHero
    {
        public string Name { get; set; } = string.Empty;
        public string Universe { get; set; } = string.Empty;
        public bool CanFly { get; set; }
    }

    [Fact]
    public void From_Classes_And_Back()
    {
        var homelander = new Hero("Homelander", "DC", true);
        var traditionalHomelander = new TraditionalHero
        { Name = "Homelander", Universe = "DC", CanFly = true };

        // Use AutoMapper to convert record into traditional
        // class and vice versa.
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Hero, TraditionalHero>();
            cfg.CreateMap<TraditionalHero, Hero>();
        });
        var mapper = config.CreateMapper();

        // Record into class
        var traditionalHero = mapper.Map<TraditionalHero>(homelander);
        Assert.Equal(homelander.Name, traditionalHero.Name);

        // Class into record
        var hero = mapper.Map<Hero>(traditionalHomelander);
        Assert.Equal(traditionalHero.Name, homelander.Name);
    }
}
