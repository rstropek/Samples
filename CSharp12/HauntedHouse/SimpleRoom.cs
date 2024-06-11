using HauntedHouse;

static partial class HauntedHouseCreator
{
    public static Dictionary<string, Room> GetRooms()
    {
        var exit = new Room(
            "Exit",
            """
            The heavy main door stands forebodingly shut, 
            its locks rusted but firm. An ancient lock seem 
            to seal it, as if it were meant to never let the night escape.
            """,
            RoomFeatures.Empty,
            // Note: Empty initializer simplifies the code
            []
        );

        var entryHall = new Room(
            "Entry Hall",
            """
            The grand hall whispers of old secrets and forgotten tales. 
            An arcane device stands in the corner, humming with an energy 
            that seems to suck in the surrounding air.
            """,
            RoomFeatures.GhostVacume,
            [] // We will fill these exits later
        );

        // Note: "var" is not possible here.
        // We could use IList, IEnumerable, etc. They would result
        // in List<T>, too.
        List<(Exit, Room)> closetExits = [(Exit.West, entryHall)];
        var closet = new Room(
            "Closet",
            """
            Cold drafts seep through the cracks of this cramped space. 
            An otherworldly chill accompanies the faint, almost 
            imperceptible, whispers as a ghostly presence lurks, guarding a rusted key.
            """,
            RoomFeatures.Ghost | RoomFeatures.Key,
            closetExits
        );

        var livingRoom = new Room(
            "Living Room",
            """
            An eerie silence pervades this room where shadows dance in 
            the flickering light. Treasures of a bygone era are scattered, 
            glinting in the half-light as if alive.
            """,
            RoomFeatures.Treasure,
            [] // We will fill these exits later
        );

        // Note that we can use collection expressions in expression
        // bodied members, too.
        List<(Exit, Room)> GetEntryHallExits() => [(Exit.East, closet), (Exit.East, livingRoom), (Exit.South, exit)];
        entryHall.Exits.AddRange(GetEntryHallExits());

        var diningRoom = new Room(
            "Dining Room",
            """
            A dimly lit room filled with the remnants of ancient feasts. 
            Cobwebs hang over the decrepit dining table, and the air is 
            thick with the scent of mildew. An empty treasure chest sits
            in the corner, its lid hanging open.
            """,
            RoomFeatures.Empty,
            [(Exit.East, livingRoom)]
        );

        // Note the spread operator here.
        livingRoom.Exits.AddRange([.. closetExits, (Exit.West, diningRoom)]);

        // Note dictionary initializer here. Collection
        // expression is not supported for dicts.
        return new()
            {
                [exit.Name] = exit,
                [entryHall.Name] = entryHall,
                [closet.Name] = closet,
                [livingRoom.Name] = livingRoom,
                [diningRoom.Name] = diningRoom,
            };
    }
}