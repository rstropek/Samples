using System.Collections.Frozen;
using System.Text.Json;
using HauntedHouse;

var hauntedHouse = HauntedHouseCreator.GetRooms();
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
