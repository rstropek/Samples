using System.Text;
using static System.Console;
using static ShoeType;

// **Wildlife Adventure**
// Learning about C# pattern matching at Techorama Jungle Edition

#region Type patterns and definite assignment
// You are a wildlife photographer and you are on an expedition
// through the jungle. You are following in the footsteps of your
// biggest idol, the famous wildlife photographer, Jim Corbett.
// Unfortunately, Jim passed away after a long, adventurous
// life. Jim has left you his notes and his camera. However, Jim
// was not a very organized person. His notes are all over the
// place and your first job is to make sense of them.
static void MakeSenseOf(object o)
{
    WriteLine($">>> Let's see what we have here: {o}");

    // Checking if the data is a string
    if (o is string)
    {
        WriteLine("""
            Ok, this looks like a word, not a number.
            Oh Jim, why did you have write in such a horrible handwriting?
            Who should be able to decipher that 🤔?
            """);
    }

    // Checking if the data is a string. If it is, extract it into a properly typed variable
    if (o is string animalName)
    {
        WriteLine($"💡 Got it! It's a {animalName}.");
    }

    // No tigers 🐯 are in the jungle, are they? We can check for a specific string.
    // 😒 Kind of annoying that we cannot reuse the animalName variable.
    // We also cannot use animalName directly as it only has a value inside
    // the code block of the previous if statement.
    if (o is "Tiger")
    {
        WriteLine("😮 WHAAAAT? There are tigers here? We have to be super careful! 🤫");
    }

    // Checking if the data is a numeric value
    if (o is int numericValue)
    {
        if (numericValue > 100)
        {
            WriteLine($"""
                🕊️ This is probably the size of a flock of {numericValue} birds that Jim saw.
                I cannot think of another animal that lives in such large groups.
                """);
        }
        else
        {
            WriteLine($"""
                🤔 Hmm, {numericValue} a rather small number.
                Maybe it's the number of legs of an animal?
                😨 Or even the number of eyes????
                Who knows what Jim found in the jungle.
                """);
        }
    }

    // Checking if the data is a tuple
    if (o is ValueTuple<int, int> photoLog)
    {
        WriteLine($"""
            👏 Jim you are my hero! 👏
            Your wrote down focal length {photoLog.Item1} and shutter speed {photoLog.Item2} 📷.
            That helps a lot!
            """);
    }

    // Default case; combining multiple type patterns
    if (!(o is string) && !(o is int) && !(o is ValueTuple<int, int>))
    {
        WriteLine("""
            🤷 Well, this one is a mystery.
            Even in the jungle, you never know what you might find!
            """);
    }

    WriteLine();
}

MakeSenseOf("Snake");
MakeSenseOf("Tiger");
MakeSenseOf(500);
MakeSenseOf(5);
MakeSenseOf((50, 100));
MakeSenseOf(true);
#endregion

WriteLine(new string('=', 80));

#region Type patterns with custom types
// Just a few days in the jungle and you already have made a ton
// of pictures. You are very excited to see what you have captured.
// You are looking at the first picture and you see a strange animal.
// It is time to take a more structured approach. Let's build
// a system to structure and classify the animals you have taken a picture of.
void Recognize(object? o)
{
    WriteLine($">>> Let's see what we have here: {o?.GetType().Name}");

    // This time we use a switch statement. Note that all the patterns shown
    // here could be used in if statements, too.
    switch (o)
    {
        // Check for a type and value by using the when clause
        case Fluffosaurus fluffosaurus when fluffosaurus.NumberOfLegs == 8:
            WriteLine("OMG, it's a Fluffosaurus with 8 legs. How lucky I am to see one 🍀😍.");
            break;
        // Check for a type (note the order of the cases)
        case Fluffosaurus:
            WriteLine($"🦖 We got ourselves a Fluffosaurus 🦖.");
            break;
        case Mammal mammal:
            WriteLine($"""
            This is a mammal {mammal.GetType().Name}.
            I know that because of its 🔊 "{mammal.Sound}" 🔊 sound.
            """);
            break;
        // Check for an interface (note the order of the cases)
        case IAnimal animal when animal.NumberOfLegs > 10:
            WriteLine($"Whoa! An animal with {animal.NumberOfLegs} legs 🦵🦵🦵, that's unusual!");
            break;
        case IAnimal:
            WriteLine("🐾 This is an animal, but I don't know which 🤷.");
            break;
        case string description when description.Trim().Length > 50:
            WriteLine("Whatever this is, it's described in great detail. I should read it later.");
            break;
        // We can also check for specific values AND null
        case null:
            WriteLine("🤔 Hmm, no animal on this picture. I wonder why I took it.");
            break;
    }

    WriteLine();
}
Recognize(new Fluffosaurus());
Recognize(new Fluffosaurus() { NumberOfLegs = 8 });
Recognize(new GigglyGlider());
Recognize(new ScreechingSunflower());
Recognize(new CentipedeCuddlebug(500));
Recognize("""
    I just saw something 😲. Its vague, inconsistent shape looks like a cross between a plant and an animal.
    It's covered in speckled neon-green and electric-blue spots that glimmer under the jungle sunlight.
    For now, let's call it the 'Speckled Snickerleaf'.
    """);
Recognize(null);
#endregion

WriteLine(new string('=', 80));

#region Recursive patterns
// Being on a wildlife expedition for such a long time is expensive. You need to make some money.
// You decide to sell your pictures to a wildlife magazine. The magazine is interested in pictures
// of animals that live in symbiosis, in particular the Fluffosaurus and the Centipede Cuddlebug.
// They will pay the most for pictures of these animals together. And for whatever reason, they are
// crazy for legs, as many legs as possible. The more legs, the more money you get. Kind of weird,
// but hey, you are not complaining.
IEnumerable<(int, string)> FindValuablePictures(IEnumerable<Symbiosis> symbioses)
    // This time we use pattern matching together with the switch expression.
    => symbioses.Select(s => s switch
        {
            // Check for a type and a value. Extract values with `var`.
            {
                Animal1: Fluffosaurus { NumberOfLegs: 8 },
                Animal2: CentipedeCuddlebug { NumberOfLegs: var legs }
            } => (100 + 5 * legs, "Checkpot! Fluffosaurus and Centipede Cuddlebug together, we get paid by the leg 🤣"),
            {
                Animal1: Fluffosaurus,
                Animal2: CentipedeCuddlebug
            } => (75, "Ok, not bad. Not so many legs, but still correct animals"),
            {
                Animal1: Fluffosaurus,
                Animal2: _, // Strictly speaking, we do not need this line. But it's sometimes good to be explicit.
                isMutualism: true
            } => (50, "No Centipede Cuddlebug, but at least an image of a Fluffosaurus symbiosis"),
            { Animal1.NumberOfLegs: > 4, isMutualism: true } => (25, "Not a perfect picture, but it is worth a little bit"),
            { Animal1: _, isMutualism: true } => (5, "Hardly worth anything, but at least it's a symbiosis"),
            _ => (0, "No symbiosis, no money"),
        });
IEnumerable<int> FindValuablePicturesWithTuples(IEnumerable<(IAnimal, IAnimal, bool)> symbioses)
    => symbioses.Select(s => s switch
        {
            (Fluffosaurus { NumberOfLegs: 8 }, CentipedeCuddlebug { NumberOfLegs: var legs }, _) => 100 + 5 * legs,
            (Fluffosaurus, CentipedeCuddlebug, _) => 75,
            (Fluffosaurus, _, true) => 50,
            ({ NumberOfLegs: > 4 }, _, true) => 25,
            (_, _, true) => 5,
            _ => 0,
        });
var animalSymbioses = new Symbiosis[]
{
    new(new Fluffosaurus() { NumberOfLegs = 8 }, new CentipedeCuddlebug(100), true),
    new(new Fluffosaurus(), new CentipedeCuddlebug(100), true),
    new(new Fluffosaurus(), new ScreechingSunflower(), true),
    new(new CentipedeCuddlebug(100), new ScreechingSunflower(), true),
    new(new ScreechingSunflower(), new GigglyGlider(), true),
    new(new GigglyGlider(), new ScreechingSunflower(), false),
};
foreach (var (money, description) in FindValuablePictures(animalSymbioses))
{
    WriteLine($"💰 {money,3} 💰: {description}");
}
foreach (var money in FindValuablePicturesWithTuples(animalSymbioses.Select(s => (s.Animal1, s.Animal2, s.isMutualism))))
{
    WriteLine($"💰 {money,3} 💰");
}
#endregion

WriteLine(new string('=', 80));

#region Relational patterns
// Jim had a large collection of cameras. He was very proud of them and he always
// took pictures with the camera that was most appropriate for the situation.
// Jim kept his cameras on a bookshelf in his hut. The bookshelf has three rows
// and six columns. Each camera has a location on the bookshelf (e.g. A1, B3, C6).
// In his notes, Jim wrote down the location of the camera he used for each picture.
// We have to write a helper method to decode the location into the camera name.
string GetCamera(string location)
{
    if (location is not { Length: 2 } || (location[0] | 0b00100000) is < 'a' or > 'c' || location[1] is < '1' or > '6')
    {
        return "🤷 No idea which camera you mean?!";
    }

    var camera = new StringBuilder();
    camera.Append(location[1] switch
    {
        <= '2' => "Snappy Snapz",
        <= '4' => "Jolly Jpegs",
        _ => "Pixel Punch"
    });
    camera.Append(' ');
    camera.Append(location[0] switch
    {
        'a' or 'A' => "Wacky Wide Angle Wonder",
        'b' or 'B' => "Delightful Detail",
        _ => "MegaPixel Mirth Machine",
    });
    return camera.ToString();
}
WriteLine($"'{GetCamera("a1")}' is the same as '{GetCamera("A1")}'");
WriteLine(GetCamera("b3"));
WriteLine(GetCamera("b"));
#endregion

WriteLine(new string('=', 80));

#region List patterns
// Some of these animals move soooo fast. If you want sharp images of them,
// you need a camera with a very fast shutter speed. Jim had so much more
// experience than you. He knew exactly which camera to use for each animal.
// You have to practice a bit and shoot the same image with different shutter
// speeds to see which one works best. You now have a bunch of photos and you
// have to find out which ones are part of the same series.
void AnalyzeShutterSpeedSeries()
{
    var shutterSpeeds = new[] { 100, 200, 500, 1000, 2000, 4000 };

    // A series of photos is a shutter speed experiment if they start with
    // 100, 200, 500, and 1000. The rest of the series is not important.
    if (shutterSpeeds is [100, 200, 500, 1000, .. var rest])
    {
        WriteLine("📸 Yes, this is a series of the same motive taken with different shutter speeds.");
        WriteLine($"The series has {rest.Length} image with shutter speeds > 1000");
    }

    // I only have a few cameras that support shutter speeds of 1/32000 and 1/16000.    
    if (shutterSpeeds is [.., 16000, 32000])
    {
        WriteLine("Wow, this was taken with a really fast camera 😮");
    }
}
AnalyzeShutterSpeedSeries();

WriteLine(new string('=', 80));

// While you are doing your practice shots, you suddenly recognize a
// Centipede Cuddlebug approaching you - and it is wearing SHOES 🤯. Who can
// believe that? You must find out what it is up to. Maybe the types of shoes
// can give you a hint?
void WhatIsTheCentipedeUpTo(ShoeType[] shoes)
{
    var message = shoes switch
    {
        [Tennis, Tennis, Tennis, ..] => "The Centipede is definitely going for a tennis match 🎾",
        [Tennis, _, Tennis, _, Tennis, ..] => "Hmm, probably tennis, but I am not sure",
        [.., FlipFlops, FlipFlops, FlipFlops] => "Whatever the Centipede does, he will end up on the beach 🏖️",
        [BalletFlats, .., BalletFlats] => $"If it starts with Ballet and ends with Ballet, it's probably Ballet 🩰",
        [Running, .. var rest, Running]
            => $"There is a {(rest.Count(r => r == Running) + 2) * 100 / (rest.Length + 2)}% chance that the Centipede is going for a run 🏃",
        [<= Hiking, .., <= Hiking] => "The Centipede is going to do sports, must be exhausing with so many legs 😅",
        [var first, .., var last] when first == last
            => $"Not sure what the animal is up to, but it's definitely something where {first} are needed",
        _ => "No idea what the Centipede is up to 🤷",
    };
    WriteLine(message);
}
WhatIsTheCentipedeUpTo(new[] { Tennis, Tennis, Tennis, Tennis });
WhatIsTheCentipedeUpTo(new[] { Tennis, Running, Tennis, Running, Tennis, Running });
WhatIsTheCentipedeUpTo(new[] { Sandals, Sandals, FlipFlops, FlipFlops, FlipFlops });
WhatIsTheCentipedeUpTo(new[] { BalletFlats, Running, Running, BalletFlats });
WhatIsTheCentipedeUpTo(new[] { Running, Running, Running, HighHeels, Running });
WhatIsTheCentipedeUpTo(new[] { Hiking, Boots, Boots, Hiking });
WhatIsTheCentipedeUpTo(new[] { Slippers, FlipFlops, Slippers });
WhatIsTheCentipedeUpTo(new[] { Tennis, Running, BalletFlats, Hiking, FlipFlops, HighHeels, Sandals, Boots, Slippers });
#endregion

WriteLine(new string('=', 80));

#region Closing
void SayGoodbye()
{
    ReadOnlySpan<char> message = new char[] { 'G', 'O', 'O', 'D', 'B', 'Y', 'E' };

    switch (message)
    {
        case "GOODBYE":
            WriteLine("🤖 Thank you so much for your attention 📸");
            break;
        default:
            WriteLine("Really, you still want to continue??");
            break;
    }
}
SayGoodbye();
#endregion

WriteLine(Figgle.FiggleFonts.Standard.Render("Pattern Matching Rockz!"));

#region Types
interface IAnimal
{
    int NumberOfLegs { get; }
    string Sound { get; }
    string Colors { get; }
}
abstract record Animal(string Description, int NumberOfLegs, string Sound, string Colors) : IAnimal;
abstract record Mammal(string Description, int NumberOfLegs, string Sound, string Colors)
    : Animal(Description, NumberOfLegs, Sound, Colors);
abstract record Bird(string Description, int NumberOfLegs, string Sound, string Colors)
    : Animal(Description, NumberOfLegs, Sound, Colors);
record Fluffosaurus() : Mammal(
    Description: """
    The Fluffosaurus is a large, ultra-fluffy animal with six legs. Despite its size, it only
    communicates with soft purring sounds, hence the name. Its fur changes color depending on
    its mood, but it's usually seen in an array of bright pastel colors.
    """,
    NumberOfLegs: 6,
    Sound: "Purr...Purr...",
    Colors: "Pastel Rainbow");
record ScreechingSunflower() : Bird(
    Description: """
    This is a peculiar bird, shaped like a sunflower. It uses its petals to fly and has two 
    small legs hidden underneath. It's named for its loud, unique screech that sounds like a trumpet.
    """,
    NumberOfLegs: 2,
    Sound: "Screeeech...Screeeech...",
    Colors: "Bright Yellow with Orange and Red");
record GigglyGlider() : Mammal(
    Description: """
    The Giggly Glider is a small, bioluminescent creature with four legs. It glides through
    the jungle treetops at night, leaving a trail of sparkling light. When it's happy or 
    excited, it emits a sound that is eerily similar to human laughter.
    """,
    NumberOfLegs: 4,
    Sound: "Heeheehee...Hahaha...",
    Colors: "Glowing Turquoise");
record CentipedeCuddlebug(int NumberOfLegs) : Animal(
    Description: """
    The Centipede Cuddlebug is a fascinating creature of the jungle, known for its whopping 100 legs!
    Despite the intimidating number of limbs, the Cuddlebug is famous for its friendly and social nature.
    It's often seen wrapping around tree branches in a large, cuddly heap, hence its name.
    """,
    NumberOfLegs: NumberOfLegs,
    Sound: "Trill...Trill...",
    Colors: "Vibrant Purple with Orange Polka Dots");

record Symbiosis(IAnimal Animal1, IAnimal Animal2, bool isMutualism);

enum ShoeType
{
    Tennis,
    Running,
    BalletFlats,
    Hiking,
    FlipFlops,
    HighHeels,
    Sandals,
    Boots,
    Slippers,
}
#endregion
