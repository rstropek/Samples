internal static class MasterData
{
    [JsonConverter(typeof(JsonStringEnumConverter<ChildFriendliness>))]
    public enum ChildFriendliness { Very, Somewhat, NotAtAll };
    public record CourseLocation(string Name, string Description, ChildFriendliness ChildFriendly);

    public static readonly CourseLocation[] Locations = [
        new("Lovely Lagoon", """
            A hidden lagoon that, legend says, is kissed by the moon's rays. It glows softly 
            at night and is surrounded by luminous plants and creatures, making it a magical 
            spot for nighttime classes.
            """,
            ChildFriendliness.Somewhat),
        new("Rainbow Reef", """
            A colorful coral reef that reflects a myriad of colors, especially when sunlight 
            filters through. It's a shallow, safe location perfect for kids, with plenty of 
            marine life to keep them intrigued.
            """,
            ChildFriendliness.Very),
        new("Whispering Waterfalls", """
            Situated at the base of gentle cascading falls, this serene spot has little underwater 
            caves and pockets of bubbles for kids to enjoy. The soothing sound of the waterfall 
            adds to the ambiance.
            """,
            ChildFriendliness.Very),
        new("Sunken Silversand Bay", """
            A sandy-bottomed bay where the sands are said to have a soft silvery hue. It's a 
            quiet bay area, protected from strong currents, making it an ideal location 
            for beginner classes.
            """,
            ChildFriendliness.Somewhat),
        new("Abyssal Atlantis Arena", """
            Dive into the ancient ruins of the Abyssal Atlantis Arena, a legendary deep-sea 
            amphitheater carved out of moonstone and surrounded by bioluminescent coral pillars. 
            The currents here are challenging, and the depths require mastery of underwater navigation.
            """,
            ChildFriendliness.NotAtAll)
    ];
}

// Generate type info for JSON serialization at compile time using source generator.
[JsonSerializable(typeof(MasterData.CourseLocation[]))]
internal partial class MasterDataJsonSerializerContext : JsonSerializerContext { }
