// ============================================================================
//  C# 15 unions shine for *recursive* closed hierarchies — e.g. an AST.
//  `Expr` references itself through its case types: the canonical
//  "discriminated union" use case.
// ============================================================================
namespace Expressions;

public record class Num(double Value);
public record class Add(Expr Left, Expr Right);
public record class Sub(Expr Left, Expr Right);
public record class Mul(Expr Left, Expr Right);
public record class Div(Expr Left, Expr Right);
public record class Neg(Expr Operand);

public union Expr(Num, Add, Sub, Mul, Div, Neg);

internal static class Program
{
    // Fold #1 — evaluate the tree to a number.
    public static double Eval(Expr e) => e switch
    {
        Num n              => n.Value,
        Add(var l, var r)  => Eval(l) + Eval(r),   // positional pattern via union
        Sub(var l, var r)  => Eval(l) - Eval(r),
        Mul(var l, var r)  => Eval(l) * Eval(r),
        Div(var l, var r)  => Eval(l) / Eval(r),
        Neg(var x)         => -Eval(x),
        null               => throw new InvalidOperationException("empty Expr"),
    };

    // Fold #2 — render the tree back to a string. Same union, different algebra.
    public static string Format(Expr e) => e switch
    {
        Num n              => n.Value.ToString(System.Globalization.CultureInfo.InvariantCulture),
        Add(var l, var r)  => $"({Format(l)} + {Format(r)})",
        Sub(var l, var r)  => $"({Format(l)} - {Format(r)})",
        Mul(var l, var r)  => $"({Format(l)} * {Format(r)})",
        Div(var l, var r)  => $"({Format(l)} / {Format(r)})",
        Neg(var x)         => $"-{Format(x)}",
        null               => throw new InvalidOperationException("empty Expr"),
    };

    private static void Main()
    {
        // (1 + 2) * -(3)  ==  -9
        Expr tree = new Mul(
            new Add(new Num(1), new Num(2)),
            new Neg(new Num(3)));

        Console.WriteLine($"{Format(tree)} = {Eval(tree)}");

        Expr poly = new Add(new Mul(new Num(3), new Num(4)), new Div(new Num(10), new Num(2)));
        Console.WriteLine($"{Format(poly)} = {Eval(poly)}");
    }
}
