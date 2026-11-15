// ============================================================================
//  A UNION MEMBER PROVIDER + a CLASS-based union (here: a record class).
//
//  Instead of public constructors, the union exposes its creation members
//  through a nested public interface named `IUnionMembers` that it implements:
//    * one static `Create(TCase)` factory per case type,
//    * a `Value` property (HasValue/TryGetValue would be optional here, too).
//
//  Why bother?
//    * the type can keep its constructor PRIVATE — every Outcome is created via
//      a union conversion (`Outcome<int> o = new Ok<int>(1);`) or Create(...),
//      so you can validate/normalise in one place,
//    * the union can be a `record class`: value equality and a readable
//      ToString() come for free (the generated `union` struct only has the
//      inherited ValueType.Equals — no `==` operator — and prints its type name).
//
//  GOTCHA I hit: inside Create, `new(value)` is target-typed to Outcome<T> —
//  which is a union — so it becomes a union conversion... which calls Create
//  again -> stack overflow. Cast to object to pick the private constructor.
// ============================================================================
using System.Runtime.CompilerServices;

namespace CustomUnions;

public sealed record Ok<T>(T Value);
public sealed record Failure(string Error);

[Union]
public sealed record class Outcome<T> : Outcome<T>.IUnionMembers
{
    public interface IUnionMembers : IUnion
    {
        static Outcome<T> Create(Ok<T> value) => new((object)value);

        // A factory can normalise input — something a plain constructor-based
        // union can't hide from callers.
        static Outcome<T> Create(Failure value) =>
            new((object)(string.IsNullOrWhiteSpace(value.Error) ? new Failure("unknown error") : value));

        new object? Value { get; }
    }

    private Outcome(object value) => Value = value;

    public object? Value { get; }
}
