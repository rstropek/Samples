// Switch between custom generic math and build-in generic math (experimental)
#define CUSTOM_GENERIC_MATH
// #define BUILTIN_GENERIC_MATH

#region Read configuration file
IConfiguration? configuration = null; // Note that nullable type is for demo purposes only
try
{
    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, false);
    configuration = builder.Build();
}
catch (InvalidDataException e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine("Expected structure of JSON:");

    // Note: Raw string literals (C# 11). Add error in appsettings.json so see its result.
    //       (see also https://slides.com/rainerstropek/csharp-11/fullscreen#/4)
    Console.WriteLine("""
                      {
                        "maxLevel": 13,       // Complexity of tree (beware of values > 13)
                        "trunkLength": 100.0, // Length of trunk
                        "boundingRect": 1     // 1 if bounding rectangle should be drawn, otherwise 0
                      }
                      """);
    return;
}

// Note: Simplified null checking (comment `return` statement above and try with/without `!!` to see difference)
//       (see also https://slides.com/rainerstropek/csharp-11/fullscreen#/3)
// Note: Generic math (IParseable); preview feature of .NET 7
//       (see also https://slides.com/rainerstropek/csharp-10-bettercode/fullscreen#/7
//       and https://devblogs.microsoft.com/dotnet/preview-features-in-net-6-generic-math/)
static T? GetConfigValue<T>(IConfiguration configuration!!, string settingName) where T: struct, IParseable<T>
{
    // Note: pattern matching
    // Note: Definite assignment
    //       (see also https://slides.com/rainerstropek/csharp-10-bettercode/fullscreen#/6)
    if (configuration[settingName] is string ml && T.TryParse(ml, CultureInfo.InvariantCulture, out var m))
    {
        return m;
    }

    return null;
}

var maxLevel = GetConfigValue<int>(configuration, "maxLevel") ?? 13;
var trunkLength = GetConfigValue<float>(configuration, "trunkLength") ?? 100;
var boundingRect = (GetConfigValue<byte>(configuration, "boundingRect") ?? 0) == 1;
#endregion

const string firstName = "Rainer";
const string lastName = "Stropek";

#region Generate hash based on user name
// Note: String interpolation enhancements
//       (see also https://slides.com/rainerstropek/csharp-10-bettercode/fullscreen#/9/1/0)
const string name = $"{firstName}{lastName}"; 

using var sha256Hash = SHA256.Create();
var hashData = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(name));
int seed = hashData
    .Chunk(4) // Note: Linq enhancements
              //       See also https://slides.com/rainerstropek/dotnet-6/fullscreen#/7
              //       and https://github.com/rstropek/Samples/blob/master/DotNet6/LinqEnhancements/Program.cs
    .Where(d => d.Length == 4)
    .Select(d => BitConverter.ToInt32(d))
    .Aggregate((sum, i) => unchecked(sum + i));
var rand = new Random(seed);
#endregion

#region Allocate memory for tree
var numberOfLines = Limit((1 << maxLevel) - 1);
Span<Line> lines = stackalloc Line[numberOfLines]; // Note: Stack allocation of record struct

/// <summary>
/// Estimates required memory and throws exception if memory limit is exceeded.
/// </summary>
static int Limit(int numberOfLines,
    // Note: New `CallerArgumentExpression` attribute
    //       (see also https://slides.com/rainerstropek/csharp-10-bettercode/fullscreen#/9)
    [CallerArgumentExpression("numberOfLines")] string? numberOfLinesExpression = null)
{
    int usedMemory;
    unsafe { usedMemory = sizeof(Line) * numberOfLines; }
    const int memoryLimit = 256 * 1024;

    if (usedMemory > memoryLimit)
    {
        throw new InvalidOperationException(
            $"Too much memory ({
                // Note: Newline in complex interpolations (C# 11)
                //       (see also https://slides.com/rainerstropek/csharp-11/fullscreen#/1)
                (numberOfLinesExpression is not null
                    // Note: Nested interpolation
                    ? $"`{numberOfLinesExpression}` = "
                    : string.Empty)
            }{numberOfLines:#,##0} lines require {usedMemory / 1024:#,##0}KB memory on the stack, which is > {memoryLimit / 1024:#,##0}KB)");
    }

    return numberOfLines;
}
#endregion

#region Generate tree geometry
var ix = 0;
static float ToRadians(float degree) => degree * (float)Math.PI / 180f;
var angleChange = ToRadians(20);
void Branch(Span<Line> lines, Vector start, float angle, float branchLength, int level)
{
    #region Some tree generation constants
    const float levelToLightnessFactor = 3f;
    const float levelToWeightFactor = 1.5f;
    const float branchShorteningFactor = 0.8f;
    const float randomAngleChangeFactor = 35f;
    const float randomBranchLengthChangeFactor = 25f;
    #endregion

    // Calculate next endpoint and add it to lines
    var end = start + new Vector(
        (float)Math.Cos(angle) * branchLength,
        (float)Math.Sin(angle) * branchLength);
    lines[ix++] = new(start, end, new(level * levelToLightnessFactor, (maxLevel - level) * levelToWeightFactor));

    // Stop if max level has been reached
    if (++level >= maxLevel) { return; }

    // Shorten next branch
    branchLength *= branchShorteningFactor;

    // Add some randomness
    // Note: Lambda improvements
    //       See also https://slides.com/rainerstropek/csharp-10-bettercode/fullscreen#/8
    // Note: Lambda functions vs. local functions (prefer local functions if possible)
    var randAngleChange = () => ToRadians(((float)rand.NextDouble() - 0.5f) * randomAngleChangeFactor);
    float randBranchLengthChange() => ((float)rand.NextDouble() - 0.5f) * randomBranchLengthChangeFactor;

    // Recursion
    Branch(lines, end, angle - angleChange + randAngleChange(), branchLength + randBranchLengthChange(), level);
    Branch(lines, end, angle + angleChange + randAngleChange(), branchLength + randBranchLengthChange(), level);
}

// Start by generating the trunk of the tree
Branch(lines, new(0f, 0f), ToRadians(-90f), trunkLength, 0);

// Aggregate lines for bounding rect and total branch length
var bounds = SumOfLines<BoundsRect>(lines);
var totalLength = SumOfLines<VectorLength>(lines);
#endregion

#region Create Skia drawing canvas
const int imageWidth = 1024;
const int imageHeight = 800;
const float baseHue = 114f;
const float maxLightness = 40f;

using var surface = SKSurface.Create(new SKImageInfo(imageWidth, imageHeight));
using var canvas = surface.Canvas;
canvas.Clear(SKColors.White);
// canvas.Translate(imageWidth / 2, imageHeight - 5);
canvas.Translate(bounds.X1 * (-1) + 10, 100 + bounds.Height);
#endregion

#region Draw branches
var colors = new Dictionary<LineSettings, SKColor>();
foreach (var line in lines)
{
    // Cache colors to reduce memory pressure
    if (!colors.TryGetValue(line.Settings, out var color))
    {
        color = SKColor.FromHsl(baseHue, 100f, Math.Min(maxLightness, line.Settings.Lightness));
        // Note: Using record struct as dictionary key
        colors[line.Settings] = color;
    }

    var linePaint = new SKPaint
    {
        Color = color,
        // Color = SKColor.FromHsl(baseHue, 100f, Math.Min(maxLightness, line.Settings.Lightness)),
        StrokeWidth = line.Settings.Width,
        IsAntialias = true,
        IsStroke = true,
    };
    canvas.DrawLine(line.Start.X, line.Start.Y, line.End.X, line.End.Y, linePaint);
}
#endregion

#region Draw bounding rectangle
if (boundingRect)
{
    var boundsPaint = new SKPaint
    {
        Color = SKColors.Red,
        StrokeWidth = 3,
        PathEffect = SKPathEffect.CreateDash(new[] { 5f, 7f }, 15),
        IsAntialias = true,
        IsStroke = true,
    };
    canvas.DrawRect(bounds.X1, bounds.Y1, bounds.Width, bounds.Height, boundsPaint);
}
#endregion

#region Draw header text
canvas.ResetMatrix();
var paint = new SKPaint
{
    TextSize = 48,
    Color = SKColor.FromHsl(baseHue, 100f, 0),
    Typeface = SKTypeface.FromFamilyName("Arial"),
    IsAntialias = true
};

// Get tree description
// Note: List pattern (C# 11) and extended property pattern (C# 10)
//       (see also https://slides.com/rainerstropek/csharp-11/fullscreen#/2/0/0)
var treeDescription = lines switch
{
    [ var b ] when (VectorLength.Zero + b).Length > 90 => "long branch",
    [ _ ] => "branch",
    [ var b, _, _ ] when (VectorLength.Zero + b).Length > 90 => "tiny tree with long trunk",
    [ _, _, _ ] => "tiny tree",
    [{ Settings.Width: > 2 }, _, _, _, .. ] => "huge tree with thick trunk",
    _ => "tree"
};

paint.TextSize = 36;
canvas.DrawText($"Hey {name}, that's your {treeDescription}:", 10, 50, paint);
paint.TextSize = 16;
canvas.DrawText($"Total branch length is {totalLength}.", 10, 80, paint);
#endregion

#region Save PNG and open it
using var image = surface.Snapshot();
using var data = image.Encode(SKEncodedImageFormat.Png, 75);
using (var file = File.OpenWrite("image.png"))
{
    file.Write(data.ToArray());
}

var p = new Process();
p.StartInfo.FileName = "explorer";
p.StartInfo.Arguments = "\"image.png\"";
p.Start();
#endregion

#region Line aggregations
T SumOfLines<T>(Span<Line> lines)
#if CUSTOM_GENERIC_MATH
    where T: ISupportAdding<T, Line, T>, IHaveZero<T>
#else
    where T : IAdditionOperators<T, Line, T>, IHaveZero<T>
#endif
{
    var result = T.Zero;
    foreach(var line in lines)
    {
        result += line;
    }

    return result;
}

interface IHaveZero<T>
{
    // Note: Static abstract members in interfaces
    //       https://slides.com/rainerstropek/csharp-10-bettercode/fullscreen#/7
    static abstract T Zero { get; }
}

#if CUSTOM_GENERIC_MATH
// Note: This method is for demo purposes only. With generic math, .NET gets
//       something like this built-in (IAdditionOperators, see also https://devblogs.microsoft.com/dotnet/preview-features-in-net-6-generic-math/)
interface ISupportAdding<TSelf, TOther, TResult>
        where TSelf : ISupportAdding<TSelf, TOther, TResult>
{
    // Note: Static abstract members in interfaces
    //       https://slides.com/rainerstropek/csharp-10-bettercode/fullscreen#/7
    static abstract TResult operator +(TSelf left, TOther right);
}
#endif

readonly record struct BoundsRect(float X1, float Y1, float X2, float Y2)
#if CUSTOM_GENERIC_MATH
    : ISupportAdding<BoundsRect, Line, BoundsRect>, IHaveZero<BoundsRect>
#else
    : IAdditionOperators<BoundsRect, Line, BoundsRect>, IHaveZero<BoundsRect>
#endif
{
    public BoundsRect() : this(0f, 0f, 0f, 0f) { }

    public static BoundsRect Zero => new();

    public static BoundsRect operator +(BoundsRect b, Line l)
    {
        if (l.Start.X < b.X1) b = b with { X1 = l.Start.X };
        if (l.End.X < b.X1) b = b with { X1 = l.End.X };
        if (l.Start.X > b.X2) b = b with { X2 = l.Start.X };
        if (l.End.X > b.X2) b = b with { X2 = l.End.X };

        if (l.Start.Y < b.Y1) b = b with { Y1 = l.Start.Y };
        if (l.End.Y < b.Y1) b = b with { Y1 = l.End.Y };
        if (l.Start.Y > b.Y2) b = b with { Y2 = l.Start.Y };
        if (l.End.Y > b.Y2) b = b with { Y2 = l.End.Y };

        return b;
    }

    public float Width => X2 - X1;
    public float Height => Y2 - Y1;
}

readonly record struct VectorLength(float Length)
#if CUSTOM_GENERIC_MATH
    : ISupportAdding<VectorLength, Line, VectorLength>, IHaveZero<VectorLength>
#else
    : IAdditionOperators<VectorLength, Line, VectorLength>, IHaveZero<VectorLength>
#endif
{
    public VectorLength() : this(0f) { }

    public static VectorLength Zero => new();

    public static VectorLength operator +(VectorLength b, Line l)
    {
        var la = l.End.X - l.Start.X;
        var lb = l.End.Y - l.Start.Y;
        return new VectorLength(b.Length + (float)Math.Sqrt(la * la + lb * lb));
    }

    public override string ToString() => Length.ToString();
}
#endregion

#region Line and Vector
// Note: record structs
//       See also https://slides.com/rainerstropek/csharp-10/fullscreen#/2
//       and https://github.com/rstropek/Samples/blob/master/DotNet6/RecordStruct/Program.cs
readonly record struct Vector(float X, float Y)
{
    public static Vector operator +(Vector first, Vector second)
        => new(first.X + second.X, first.Y + second.Y);
}

readonly record struct LineSettings(float Lightness, float Width)
{
    public LineSettings() : this(50, 1) { }
}

readonly record struct Line(Vector Start, Vector End, LineSettings Settings)
{
    public Line() : this(new(0f, 0f), new(0f, 0f)) { }
    public Line(Vector Start, Vector End) : this(Start, End, new()) { }
}
#endregion
