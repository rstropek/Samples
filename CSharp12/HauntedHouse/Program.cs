using System.Collections.Frozen;
using System.Text.Json;

var hauntedHouse = HauntedHouse.GetRooms();
// foreach (var room in hauntedHouse.Values)
// {
//     Console.WriteLine(room);
//     Console.WriteLine();
// }

var parser = new HauntedHouseParser();
hauntedHouse = parser.Parse("""
    [
        {
            "Name": "Exit",
            "Description": "The heavy main door stands forebodingly shut, its locks rusted but firm. An ancient lock seem to seal it, as if it were meant to never let the night escape."
        },
        {
            "Name": "Entry Hall",
            "Description": "The grand hall whispers of old secrets and forgotten tales. An arcane device stands in the corner, humming with an energy that seems to suck in the surrounding air.",
            "Features": 8,
            "Exits": {
                "West": "Closet",
                "East": "Living Room",
                "South": "Exit"
            }
        },
        {
            "Name": "Closet",
            "Description": "Cold drafts seep through the cracks of this cramped space. An otherworldly chill accompanies the faint, almost imperceptible, whispers as a ghostly presence lurks, guarding a rusted key.",
            "Features": 9,
            "Exits": {
                "East": "Entry Hall"
            }
        },
        {
            "Name": "Living Room",
            "Description": "An eerie silence pervades this room where shadows dance in the flickering light. Treasures of a bygone era are scattered, glinting in the half-light as if alive.",
            "Features": 2,
            "Exits": {
                "West": "Entry Hall"
            }
        }
    ]
    """);
foreach (var room in hauntedHouse.Values)
{
    Console.WriteLine(room);
    Console.WriteLine();
}

[Flags]
enum RoomFeatures
{
    Empty,
    Ghost = 0b0000_0001,
    Treasure = 0b0000_0010,
    HumorousElement = 0b0000_0100,
    GhostVacume = 0b0000_1000,
    BagOfHolding = 0b0001_0000,
    Key = 0b0010_0000,
}

[Flags]
enum Exit
{
    North,
    South,
    East,
    West,
}

// Exercise
class Room(string name, string description, RoomFeatures features, List<(Exit, Room)> exits, bool hasWindows = false)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public RoomFeatures Features { get; } = features;
    public List<(Exit Direction, Room TargetRoom)> Exits { get; } = exits;

    public Room(string name) : this(name, string.Empty, RoomFeatures.Empty, []) { }

    public override string ToString()
    {
        // Try using primary constructor parameters here
        return $"""
        Name: {Name}
        Description: {Description[..10]}...
        Features: {Features}
        Exits: {(Exits.Count != 0
                ? string.Join(", ", Exits.Select(e => $"{e.Direction} to {e.TargetRoom.Name}"))
                : "None")}
        """;
    }
}

class HauntedHouseParser
{
    private record RoomParseDto(string Name, string? Description, RoomFeatures? Features, Dictionary<string, string>? Exits);

    public Dictionary<string, Room> Parse(string json)
    {
        var rooms = JsonSerializer.Deserialize<List<RoomParseDto>>(json) ?? throw new ArgumentException("Invalid JSON");

        var house = new Dictionary<string, Room>();
        foreach (var room in rooms)
        {
            var features = room.Features ?? RoomFeatures.Empty;
            var description = room.Description ?? string.Empty;
            var newRoom = new Room(room.Name, description, features, []);
            house.Add(room.Name, newRoom);
        }

        foreach (var room in rooms)
        {
            var currentRoom = house[room.Name];
            if (room.Exits is null) { continue; }

            foreach (var (direction, targetRoomName) in room.Exits)
            {
                var targetRoom = house[targetRoomName];
                currentRoom.Exits.Add((Enum.Parse<Exit>(direction), targetRoom));
            }
        }

        return house;
    }
}
