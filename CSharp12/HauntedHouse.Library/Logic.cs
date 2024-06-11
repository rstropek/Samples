using System.Text.Json;

namespace HauntedHouse;

[Flags]
public enum RoomFeatures
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
public enum Exit
{
    North,
    South,
    East,
    West,
}

// Exercise
public class Room(string name, string description, RoomFeatures features, List<(Exit, Room)> exits, bool hasWindows = false)
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
        Description: {Description}...
        Features: {Features}
        Exits: {(Exits.Count != 0
                ? string.Join(", ", Exits.Select(e => $"{e.Direction} to {e.TargetRoom.Name}"))
                : "None")}
        """;
    }
}

public class HauntedHouseParser
{
    private record class RoomParseDto(string Name, string? Description, RoomFeatures? Features, Dictionary<string, string>? Exits);

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

            foreach (var ex in room.Exits)
            {
                var targetRoom = house[ex.Value];
                currentRoom.Exits.Add(((Exit)Enum.Parse(typeof(Exit), ex.Key), targetRoom));
            }
        }

        return house;
    }
}
