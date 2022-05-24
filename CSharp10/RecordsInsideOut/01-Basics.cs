namespace RecordsInsideOut;

public class Basics
{
    #region Naive hero class
    private class HeroNaive
    {
        public HeroNaive() { }
        public string Name { get; set; } = string.Empty;
        public string Universe { get; set; } = string.Empty;
        public bool CanFly { get; set; }
    }

    [Fact]
    public void Work_With_Naive_Hero()
    {
        // We can initialize props when creating the object.
        // We cannot enforce initialization if props (at least in .NET 6)
        var h = new HeroNaive()
        {
            Name = "Homelander",
            Universe = "DC",
            CanFly = true
        };
        Assert.Equal("Homelander", h.Name);

        // Class is mutable. Should it be?
        h.Name = "Stormfront";
        Assert.Equal("Stormfront", h.Name);
    }
    #endregion

    #region Hero with ctor
    private class HeroWithCtor
    {
        // Let's add a constructor to enforce initialization of certain props
        public HeroWithCtor(string name, string universe, bool canFly)
            => (Name, Universe, CanFly) = (name, universe, canFly);

        // We also make the props read-only by removing the setters
        public string Name { get; }
        public string Universe { get; }
        public bool CanFly { get; }
    }

    [Fact]
    public void Work_With_Ctor_Hero()
    {
        var h1 = new HeroWithCtor("The Deep", "DC", false);
        Assert.Equal("DC", h1.Universe);

        // That doesn't work as ctor parameters are mandatory
        // var h2 = new HeroWithCtor() { Name = "The Deep", Universe = "DC" };

        // That doesn't work either as class is immutable
        // h.Name = "Starlight";
    }
    #endregion

    #region Comparing heroes
    // Wouldn't it be nice if we could easily compare heroes based on the props'
    // contents, not based on the hero objects' identities? Let's add IEquatable support
    // for that.
    private class CompareableHero : IEquatable<CompareableHero>
    {
        public CompareableHero(string name = "", string universe = "", bool canFly = false)
            => (Name, Universe, CanFly) = (name, universe, canFly);

        // Note that we switch from read-only props to init-only. By giving
        // default values to ctor parameters, the user of the record can choose
        // between ctor params and prop initializers.
        public string Name { get; init; }
        public string Universe { get; init; }
        public bool CanFly { get; init; }
        public bool Equals(CompareableHero? other)
            => other != null && Name == other.Name && Universe == other.Universe && CanFly == other.CanFly;
        public override bool Equals(object? obj)
            => obj != null && obj is CompareableHero h && Equals(h);

        // We also implement GetHashCode for quick comparison, hash-based collections etc.
        public override int GetHashCode() => HashCode.Combine(Name, Universe, CanFly);
    }

    [Fact]
    public void Work_With_Comparable_Hero()
    {
        var h1 = new CompareableHero("Queen Maeve", "DC", false);
        var h2 = new CompareableHero()
        {
            Name = "Queen Maeve",
            Universe = "DC",
            CanFly = false
        };

        Assert.Equal(h1, h2);
    }
    #endregion

    #region Cloning heroes
    private class CloneableHero : IEquatable<CloneableHero>
    {
        public CloneableHero(string name = "", string universe = "", bool canFly = false)
            => (Name, Universe, CanFly) = (name, universe, canFly);
        public string Name { get; init; }
        public string Universe { get; init; }
        public bool CanFly { get; init; }
        public bool Equals(CloneableHero? other)
            => other != null && Name == other.Name && Universe == other.Universe && CanFly == other.CanFly;
        public override bool Equals(object? obj)
            => obj != null && obj is CloneableHero h && Equals(h);
        public override int GetHashCode() => HashCode.Combine(Name, Universe, CanFly);

        // We add a convenience function to the class that makes it simple to clone objects
        public CloneableHero Clone() => new(Name, Universe, CanFly);
    }

    [Fact]
    public void Work_With_Clonable_Hero()
    {
        var h1 = new CloneableHero("Black Noir", "DC", false);
        var h2 = h1.Clone();
        Assert.Equal(h1, h2);

        // This doesn't work because Name is init-only property
        // h2.Name = "Lamplighter";
    }
    #endregion

    #region Deconstruction
    private class DeconstructableHero : IEquatable<DeconstructableHero>
    {
        public DeconstructableHero(string name = "", string universe = "", bool canFly = false)
            => (Name, Universe, CanFly) = (name, universe, canFly);
        public string Name { get; init; }
        public string Universe { get; init; }
        public bool CanFly { get; init; }
        public bool Equals(DeconstructableHero? other)
            => other != null && Name == other.Name && Universe == other.Universe && CanFly == other.CanFly;
        public override bool Equals(object? obj)
            => obj != null && obj is DeconstructableHero h && Equals(h);
        public override int GetHashCode()  => HashCode.Combine(Name, Universe, CanFly);
        public DeconstructableHero Clone() => new(Name, Universe, CanFly);

        // We add a deconstructor to the class.
        public void Deconstruct(out string name, out string universe, out bool canFly)
            => (name, universe, canFly) = (Name, Universe, CanFly);
    }

    [Fact]
    public void Work_With_Deconstructable_Hero()
    {
        var h1 = new DeconstructableHero("Translucent", "DC", false);
        var (name, _, canFly) = h1;
        Assert.Equal("Translucent", name);
        Assert.False(canFly);
    }
    #endregion

    #region "Harry Potter" programming with records
    // RECORDS SOLVES ALL THAT PROBLEMS!
    // Use a disassembler like dnSpy to look at generated class
    private record Hero(string Name = "", string Universe = "", bool CanFly = false);

    [Fact]
    public void Work_With_Hero()
    {
        // Creation using ctor
        var h1 = new Hero("Homelander", "DC", true);

        // Creation with property initializations
        var h2 = new Hero { Name = "The Deep", Universe = "DC" };

        // Deconstruction
        var (name, _, canFly) = h2;
        Assert.Equal("The Deep", name);
        Assert.False(canFly);

        // Cloning with changes
        var h3 = h1 with { Name = "Stormfront" };
        Assert.Equal("DC", h3.Universe);

        // Creating and initializing collections
        Hero tick;
        _ = new[]
        {
            tick = new Hero("The Tick", CanFly: false),
            tick with { Name = "The Terror" }
        };

        // Value-based equality
        var anotherH1 = new Hero("Homelander", "DC", true);
        Assert.Equal(h1, anotherH1);

        // Instances of records are still immutable, so this doesn't work:
        // h1.Name = "Starlight";
    }
    #endregion
}
