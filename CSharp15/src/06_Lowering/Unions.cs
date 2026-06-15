// ============================================================================
//  These tiny types exist to be DECOMPILED. Build this project, then run
//  ./decompile.sh (or see TALK-CONCEPT.md) to inspect what the compiler emits.
// ============================================================================
namespace Lowering;

public record class Circle(double Radius);
public record class Rectangle(double Width, double Height);

// A union declaration ...
public union Shape(Circle, Rectangle);

// ... is lowered by the C# 15 compiler to roughly:
//
//   [System.Runtime.CompilerServices.Union]
//   public struct Shape : System.Runtime.CompilerServices.IUnion
//   {
//       public object? Value { get; }
//       public Shape(Circle value)    => Value = value;
//       public Shape(Rectangle value) => Value = value;
//   }
//
// The runtime contract (System.Runtime.CompilerServices) is simply:
//
//   public interface IUnion { object? Value { get; } }
//   [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
//   public sealed class UnionAttribute : Attribute { }

public static class Consumer
{
    // Watch how the implicit conversion and the exhaustive switch lower.
    // We INTENTIONALLY omit a `null` arm here: this is the exhibit that shows
    // the compiler's safety net for the default/empty union — the lowering
    // emits a `ThrowSwitchExpressionException` fallback (see TALK-CONCEPT).
    // On stricter previews that net is reported as CS8655; we suppress it so
    // the teaching IL stays intact.
#pragma warning disable CS8655 // non-exhaustive over null (the empty union) — intentional
    public static double Area(Shape shape) => shape switch
    {
        Circle c    => System.Math.PI * c.Radius * c.Radius,
        Rectangle r => r.Width * r.Height,
    };
#pragma warning restore CS8655

    public static Shape MakeCircle(double r) => new Circle(r); // implicit conversion
}
