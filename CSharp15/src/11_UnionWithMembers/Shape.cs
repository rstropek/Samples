// ============================================================================
//  A union can carry its OWN members. The shorthand `union X(...)` accepts a
//  body, just like a struct — so you can add methods, computed properties, and
//  an explicit `ToString()` (recall: the generated ToString is NOT forwarded
//  to the case value, so overriding it is the idiomatic fix for that gotcha).
//
//  Inside the body, switch over `this` (the UNION) so that union exhaustiveness
//  applies — matching the case types is then enough. (Switching over the raw
//  `Value` property, typed `object?`, would NOT be exhaustive and warns CS8509
//  'not null not covered'. The `null` arm handles the default/empty union.)
// ============================================================================
namespace UnionMembers;

public record class Circle(double Radius);
public record class Rectangle(double Width, double Height);
public record class Triangle(double Base, double Height);

public union Shape(Circle, Rectangle, Triangle)
{
    // Computed property — custom logic over the cases.
    public double Area => this switch
    {
        Circle c    => Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
        Triangle t  => 0.5 * t.Base * t.Height,
        null        => 0,
    };

    public bool IsDegenerate => Area == 0;

    // A method on the union.
    public Shape Scaled(double factor) => this switch
    {
        Circle c    => new Circle(c.Radius * factor),
        Rectangle r => new Rectangle(r.Width * factor, r.Height * factor),
        Triangle t  => new Triangle(t.Base * factor, t.Height * factor),
        null         => this,
    };

    // Custom ToString — fixes the "ToString is not forwarded" gotcha.
    public override string ToString() => this switch
    {
        Circle c    => $"Circle(r={c.Radius})",
        Rectangle r => $"Rectangle({r.Width}x{r.Height})",
        Triangle t  => $"Triangle(b={t.Base}, h={t.Height})",
        null        => "Shape(<uninitialised>)",
    };

    // A static factory living on the union itself.
    public static Shape Unit => new Circle(1);
}
