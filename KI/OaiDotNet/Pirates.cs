namespace ArtificialPirates;

public record Pirate(string Name, string Hat, string ClothingColor, string Weapon, string EyePatch, string SpecialFeature)
{
    public override string ToString()
    {
        return $"""
            # {Name}

            * Hat: {Hat}
            * Clothing color: {ClothingColor}
            * Weapon: {Weapon}
            * Eye patch: {EyePatch}
            * Special feature: {SpecialFeature}            
            """;
    }
}

public static class Pirates
{
    public static readonly List<Pirate> FamousPirates =
    [
        new(
            "Captain Redbeard",
            "Tricorn hat with skull emblem",
            "Red",
            "Cutlass",
            "Black on right eye",
            "Scar across left cheek"
        ),
        new(
            "Black Jack",
            "Bandana with pirate insignia",
            "Black",
            "Flintlock pistol",
            "Brown on left eye",
            "Golden tooth"
        ),
        new(
            "One-Eyed Willie",
            "Wide-brimmed hat with feather",
            "Blue",
            "Hook hand",
            "Black on right eye",
            "Wooden leg"
        ),
        new(
            "Mad Mary",
            "Pirate hat with red ribbon",
            "Green",
            "Dagger",
            "Black on left eye",
            "Tattoo of an anchor on arm"
        ),
        new(
            "Silver Sam",
            "Leather hat with silver buckle",
            "Silver",
            "Rapier",
            "Black on right eye",
            "Silver earring"
        )
    ];
}
