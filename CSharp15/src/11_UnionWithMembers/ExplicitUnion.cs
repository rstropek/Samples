// ============================================================================
//  The shorthand is just sugar. A hand-written struct that carries [Union],
//  implements IUnion, and exposes the case constructors is ALSO a union: it
//  gets implicit conversions, pattern unwrapping, and exhaustiveness — and you
//  control every member (storage, ToString, operators, ...).
// ============================================================================
using System.Runtime.CompilerServices;

namespace UnionMembers;

public record class Ok(int Code);
public record class Fail(string Reason);

[Union]
public struct HttpOutcome : IUnion
{
    public object? Value { get; }
    public HttpOutcome(Ok ok)    => Value = ok;
    public HttpOutcome(Fail err) => Value = err;

    public bool IsSuccess => Value is Ok;

    public override string ToString() => this switch
    {
        Ok o   => $"OK {o.Code}",
        Fail f => $"FAIL ({f.Reason})",
        null   => "OUTCOME <uninitialised>",
    };
}

internal static class ExplicitDemo
{
    public static void Run()
    {
        Console.WriteLine();
        HttpOutcome ok = new Ok(200);          // implicit conversion works on the explicit struct
        HttpOutcome bad = new Fail("timeout");
        Console.WriteLine($"{ok} (success={ok.IsSuccess})");
        Console.WriteLine($"{bad} (success={bad.IsSuccess})");
    }
}
