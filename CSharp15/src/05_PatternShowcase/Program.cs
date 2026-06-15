// ============================================================================
//  Every C# pattern form works "through" a union — the value is unwrapped to
//  its case instance before the pattern is applied.
// ============================================================================
namespace Patterns;

public record class Circle(double Radius);
public record class Rectangle(double Width, double Height);
public record class Triangle(double Base, double Height);

public union Shape(Circle, Rectangle, Triangle);

internal static class Program
{
    // type + positional + property + relational + logical + `when`
    public static string Classify(Shape shape) => shape switch
    {
        Circle { Radius: 0 }                    => "degenerate circle",
        Circle { Radius: < 1 } c                => $"tiny circle r={c.Radius}",
        Circle c                                => $"circle r={c.Radius}",
        Rectangle { Width: var w, Height: var h } when w == h => $"square {w}x{h}",
        Rectangle r                             => $"rectangle {r.Width}x{r.Height}",
        Triangle { Base: > 0, Height: > 0 } t   => $"triangle area={0.5 * t.Base * t.Height}",
        Triangle                                => "degenerate triangle",
        null                                    => "<uninitialised shape>",
    };

    public static double Area(Shape shape) => shape switch
    {
        Circle c    => Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
        Triangle t  => 0.5 * t.Base * t.Height,
        null        => throw new InvalidOperationException("uninitialised Shape"),
    };

    // Switching over a *tuple of unions* — exhaustiveness across the product.
    public static string Compare(Shape a, Shape b) => (a, b) switch
    {
        (Circle, Circle)        => "two circles",
        (Rectangle, Rectangle)  => "two rectangles",
        (Triangle, Triangle)    => "two triangles",
        _                       => "mixed pair",
    };

    private static void Main()
    {
        Shape[] shapes =
        [
            new Circle(0),
            new Circle(0.5),
            new Circle(3),
            new Rectangle(4, 4),
            new Rectangle(4, 6),
            new Triangle(3, 8),
        ];

        foreach (var s in shapes)
            Console.WriteLine($"{Classify(s),-28} area={Area(s):F2}");

        Console.WriteLine();
        Console.WriteLine(Compare(new Circle(1), new Circle(2)));
        Console.WriteLine(Compare(new Circle(1), new Triangle(1, 1)));
    }
}
