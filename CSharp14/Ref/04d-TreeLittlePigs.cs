namespace Ref;

public static class PigsDemo
{
    public static void Demo()
    {
        Console.WriteLine("\n🐷🐷🐷 with 🛖🛖🏠");
        Span<PigHouse> pigs = stackalloc[] { PigHouse.Straw, PigHouse.Sticks, PigHouse.Bricks };
        Span<HouseState> houses = stackalloc[] { HouseState.Unknown, HouseState.Unknown, HouseState.Unknown };
        var tlp = new ThreeLittlePigs(pigs, houses);
        // Pig1 maybe changes its mind...
        // pigs[1] = PigHouse.Bricks;
        tlp.HuffPuffBlow();
        foreach (var house in houses) { Console.WriteLine(house); }

        Console.WriteLine("\n🐷🐷🐷 are smarter now: 🏠🏠🏠");
        ReadOnlySpan<PigHouse> pigsAreSmaterNow = stackalloc[] { PigHouse.Bricks, PigHouse.Bricks, PigHouse.Bricks };
        tlp.SetPigs(pigsAreSmaterNow);
        tlp.HuffPuffBlow();
        foreach (var house in houses) { Console.WriteLine(house); }
    }
}

enum PigHouse : byte
{
    Straw,
    Sticks,
    Bricks
}

enum HouseState : byte
{
    Unknown,
    FallenDown,
    Intact
}

// Note that ThreeLittlePigs cannot be a readonly ref struct as
// not all members are readonly refs.
ref struct ThreeLittlePigs
{
    // Note that the pigs have to be ref readonly as we accept a ReadOnlySpan
    // in method SetPigs and in the ctor. Try to remove readonly and see
    // what's going to happen.
    // Note that the pigs cannot be readonly ref readonly as we can
    // change the references in the method SetPigs. Try to change one
    // pig to readonly ref readonly and see what's going to happen.
    ref readonly PigHouse pig1; // reference to a readonly PigHouse
    ref readonly PigHouse pig2;
    ref readonly PigHouse pig3;

    // Note that houses cannot be ref readonly as we manipulate the
    // status of the houses in a method. Try to add readonly and see
    // what's going to happen.
    // Note that houses can be readonly ref as we only set their content,
    // we never change the ref itself.
    readonly ref HouseState house1; // this reference cannot change after it has been set, but
                                    // the content it points to can change
    readonly ref HouseState house2;
    readonly ref HouseState house3;

    // Yes, `readonly ref readonly SomeType someField` 😅 would be possible, too.
    // This would be a readonly reference to a readonly field of a struct.

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreeLittlePigs"/> struct.
    /// </summary>
    /// <param name="pigs">Represents the pigs with their houses</param>
    /// <param name="houses">Represents the statuses of the houses of the pigs</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="pigs"/> or <paramref name="houses"/> do not contain 
    /// greater than or equal 3 elements.
    /// </exception>
    /// <remarks>
    /// The pigs' houses (<paramref name="pigs"/>) are only read while the 
    /// statuses of the houses (<paramref name="houses"/>) are manipulated.
    /// </remarks>
    public ThreeLittlePigs(ReadOnlySpan<PigHouse> pigs, Span<HouseState> houses)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(houses.Length, 3, nameof(houses));

        SetPigs(pigs);

        house1 = ref houses[0];
        house2 = ref houses[1];
        house3 = ref houses[2];
    }

    /// <summary>
    /// Sets the pigs with their houses to new references
    /// </summary>
    /// <param name="pigs">Represents the pigs with their houses</param>
    /// <exception cref="ArgumentException">
    /// <paramref name="pigs"/> or <paramref name="houses"/> do not contain 
    /// greater than or equal 3 elements.
    /// </exception>
    public void SetPigs(ReadOnlySpan<PigHouse> pigs)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pigs.Length, 3, nameof(pigs));

        pig1 = ref pigs[0];
        pig2 = ref pigs[1];
        pig3 = ref pigs[2];
    }

    public readonly void HuffPuffBlow()
    {
        // We can make the method readonly as we don't change the state of its fields.
        // We only change the content(!) of the references.

        HuffPuffBlow(pig1, ref house1);
        HuffPuffBlow(pig2, ref house2);
        HuffPuffBlow(pig3, ref house3);
    }

    private static void HuffPuffBlow(PigHouse pig, ref HouseState house)
        => house = pig switch
        {
            PigHouse.Straw => HouseState.FallenDown,
            PigHouse.Sticks => HouseState.FallenDown,
            PigHouse.Bricks => HouseState.Intact,
            _ => throw new InvalidOperationException()
        };
}