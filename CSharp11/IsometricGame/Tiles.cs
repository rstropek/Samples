using System.Diagnostics.CodeAnalysis;
using SkiaSharp;

#region Tiles
/// <summary>
/// Contains themed tile collections.
/// </summary>
public static class TileTypes
{
    public static readonly Tile[] Arctic = new Tile[]
    {
        new(string.Empty, TileCategory.None),
        new("Arctic (2).png", TileCategory.Snow),
        new("Arctic (3).png", TileCategory.Snow | TileCategory.CanHavePoints),
        new("Arctic (4).png", TileCategory.Snow),
        new("Arctic (5).png", TileCategory.Snow),
        new("Arctic (7).png", TileCategory.Snow | TileCategory.Bridge),
        new("Arctic (8).png", TileCategory.Snow | TileCategory.Bridge),
        new("Arctic (14).png", TileCategory.Snow),
        new("Arctic (15).png", TileCategory.Snow),
        new("Arctic (16).png", TileCategory.Snow),
        new("Arctic (16).png", TileCategory.Snow),
    };

    public static readonly Tile[] Winter = new Tile[]
    {
        new(string.Empty, TileCategory.None),
        new("Snow (27).png", TileCategory.Green),
        new("Snow (28).png", TileCategory.Green | TileCategory.CanHavePoints),
        new("Snow (29).png", TileCategory.Green),
        new("Snow (32).png", TileCategory.Snow),
        new("Snow (33).png", TileCategory.Snow | TileCategory.CanHavePoints),
        new("Snow (34).png", TileCategory.Snow),
        new("Snow (37).png", TileCategory.Bridge),
        new("Snow (38).png", TileCategory.Bridge),
    };

    public static readonly Tile[] Halloween = new Tile[]
    {
        new(string.Empty, TileCategory.None),
        new("Ghost (27).png", TileCategory.Dark),
        new("Ghost (28).png", TileCategory.Dark),
        new("Ghost (31).png", TileCategory.Dark),
        new("Ghost (32).png", TileCategory.Dark  | TileCategory.CanHavePoints),
        new("Ghost (33).png", TileCategory.Pink),
        new("Ghost (36).png", TileCategory.Pink  | TileCategory.CanHavePoints),
        new("Ghost (37).png", TileCategory.Bridge),
        new("Ghost (38).png", TileCategory.Bridge),
    };

    /// <summary>
    /// Gets a random tile from a themed tile collection.
    /// </summary>
    public static Tile GetRandom(Tile[] theme)
    {
        ArgumentNullException.ThrowIfNull(theme);

        // Note the use of Random.Shared here. It has been around since .NET 6, but
        // still many people do new Random().Next().
        var tile = theme[Random.Shared.Next(theme.Length)];
        return tile with { Decoration = DecorationTypes.GetRandom(tile.Category) };
    }

    /// <summary>
    /// Get tile by image number from a themed tile collection.
    /// </summary>
    public static Tile GetByNumber(Tile[] theme, int number)
    {
        ArgumentNullException.ThrowIfNull(theme);
        var tile = theme.First(t => t.Type.EndsWith($"({number}).png"));
        return tile with { Decoration = DecorationTypes.GetRandom(tile.Category) };
    }
}

[Flags]
public enum TileCategory
{
    None = 0,
    Snow = 0b1,
    Bridge = 0b10,
    CanHavePoints = 0b1_00,
    Green = 0b10_00,
    Dark = 0b1_00_00,
    Pink = 0b10_00_00,
}

public static class TileArrayExtensions
{
    public static Tile[] CloneTiles(this Tile[] tiles) => tiles.Select(t => t with { }).ToArray();
}

// Note new generic parsable in the following type.

/// <summary>
/// Represents a tile.
/// </summary>
public record Tile(string Type, TileCategory Category) : IParsable<Tile>, ISpanParsable<Tile>
{
    public bool IsSnow => Category.HasFlag(TileCategory.Snow);
    public bool IsBridge => Category.HasFlag(TileCategory.Bridge);
    public bool CanHaveDecoration => Category.HasFlag(TileCategory.CanHavePoints);
    public bool IsEmpty => Type == string.Empty;

    public Decoration? Decoration { get; set; }

    public void Draw(Spritesheet sprites, SKCanvas canvas, SKSize? destSize = null)
    {
        if (Type == string.Empty) { return; }

        canvas.Save();
        canvas.DrawCenteredBitmap(sprites, Type, destSize);
        if (Decoration != null)
        {
            canvas.Translate(0f, Decoration.TranslationY);
            canvas.DrawCenteredBitmap(sprites, Decoration.Type);
        }
        canvas.Restore();
    }
    
    public bool CanGoTo => !string.IsNullOrEmpty(Type) && (Decoration == null || Decoration.Reachable);

    #region Parsable
    public static Tile Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (TryParse(s, provider, out var tile))
        {
            return tile;
        }

        throw new FormatException($"Invalid tile: {s.ToString()}");
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Tile result)
    {
        // Note using of list pattern in the following code.
        // See also https://slides.com/rainerstropek/csharp-11/fullscreen#/4
        
        result = s switch
        {
            ['W', .. var num] => TileTypes.GetByNumber(TileTypes.Winter, int.Parse(num)),
            ['A', .. var num] => TileTypes.GetByNumber(TileTypes.Arctic, int.Parse(num)),
            ['H', .. var num] => TileTypes.GetByNumber(TileTypes.Halloween, int.Parse(num)),
            _ => null!
        };
        return result != null;
    }

    public static Tile Parse(string s, IFormatProvider? provider)
        => Parse(s.AsSpan(), provider);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Tile result)
        => TryParse(s.AsSpan(), provider, out result);
    #endregion
}
#endregion

#region Decorations
/// <summary>
/// Contains decoration collections.
/// </summary>
public static class DecorationTypes
{
    public static readonly Decoration[] All = new Decoration[]
    {
        new("Snow (11).png", -40f, true, 0),
        new("Snow (17).png", -40f, true, 0),
        new("Snow (23).png", -190f, false, 0),
        new("Snow (24).png", -220f, false, 0),
    };

    public static Decoration? GetRandom(TileCategory category)
    {
        // If category allows a point chip, add one.
        if (category.HasFlag(TileCategory.CanHavePoints))
        {
            // Generate random value
            var value = Random.Shared.Next(9);

            // Build image frame name
            string type;
            if (category.HasFlag(TileCategory.Dark))
            {
                // Note new .NET possibility to write multi-line string interpolations.
                // See also https://slides.com/rainerstropek/csharp-11/fullscreen#/2

                type = $"Ghost ({
                    // The first bonus chip with value 1 in the Ghost image collection
                    // has the name "Ghost (39).png". Therefore, we start with 39
                    // and add the value. Same is done for other image collections below.
                    39 + value
                    }).png";
            } else if (category.HasFlag(TileCategory.Pink))
            {
                type = $"Ghost ({59 + value}).png";
            } else if (category.HasFlag(TileCategory.Green))
            {
                type = $"Snow ({39 + value}).png";
            } else
            {
                type = $"Snow ({49 + value}).png";
            }

            // Return point chip
            return new(type, -40f, true, value + 1);
        }
        else if (!category.HasFlag(TileCategory.Bridge) && Random.Shared.Next(0, 3) >= 2)
        {
            // We add a random decoration to ~1/3rd of the tiles that are not bridges.
            // A tree on a bridge would look weird ;-)
            // Note: No need to clone decoration here, because we don't modify it.
            return All[Random.Shared.Next(All.Length)];
        }

        // No decoration
        return null;
    }
}

/// <summary>
/// Represents a decoration.
/// </summary>
public record Decoration(string Type, float TranslationY, bool Reachable, int Value)
{
    public bool Pickable => Value > 0;
};
#endregion
