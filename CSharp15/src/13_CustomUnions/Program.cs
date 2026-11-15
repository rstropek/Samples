// ============================================================================
//  C# 15 — Custom unions: everything beyond the `union X(...)` shorthand.
//    (1) ValueOption<T>  — a non-boxing [Union] struct (HasValue/TryGetValue)
//    (2) Outcome<T>      — a record-class union with a member provider
//  Both get the full union treatment: implicit conversions, pattern
//  unwrapping and exhaustiveness.
// ============================================================================
using CustomUnions;

// --- (1) Non-boxing struct union ---------------------------------------------
Console.WriteLine("== (1) ValueOption<T> — non-boxing union ==");
ValueOption<int> some = 42;                 // implicit conversion via the constructor
ValueOption<int> none = None.Instance;
ValueOption<int> empty = default;
foreach (var o in new[] { some, none, empty })
    Console.WriteLine($"{o,-8} -> {Probes.Describe(o)}");

// Allocation comparison: 1000 conversions + one type-pattern test each.
const int N = 1_000;
Measure("shorthand union (boxes int)", () => Probes.SumBoxed(N));
Measure("ValueOption<int> (TryGetValue)", () => Probes.SumValueOption(N));

// --- (2) Member provider + record-class union --------------------------------
Console.WriteLine();
Console.WriteLine("== (2) Outcome<T> — member provider, record class ==");
Outcome<int> a = new Ok<int>(7);            // conversion goes through IUnionMembers.Create
Outcome<int> b = new Ok<int>(7);
Outcome<int> c = new Failure("   ");        // normalised by the factory
Console.WriteLine($"a = {a}");               // record ToString, not just the type name
Console.WriteLine($"a == b: {a == b}");      // record value equality
Console.WriteLine($"c = {Render(c)}");
// new Outcome<int>(...)                     // error: the constructor is private

// A class-based union can also be a null REFERENCE; the `null` pattern covers
// both a null reference and a null Value.
Outcome<int>? missing = null;
Console.WriteLine($"missing = {Render(missing)}");

static string Render(Outcome<int>? o) => o switch
{
    Ok<int>(var v)      => $"OK {v}",
    Failure(var error)  => $"FAILED: {error}",
    null                => "<null>",
};

static void Measure(string label, Func<int> body)
{
    body();                                  // warm up (JIT, statics)
    long before = GC.GetAllocatedBytesForCurrentThread();
    int result = body();
    long bytes = GC.GetAllocatedBytesForCurrentThread() - before;
    Console.WriteLine($"{label,-32} sum={result}  allocated={bytes:N0} bytes");
}

// Kept in a named class so ./decompile.sh can find the methods in the IL.
static class Probes
{
    public static string Describe(ValueOption<int> o) => o switch
    {
        int v  => $"some int {v}",           // compiled to TryGetValue(out int)
        None   => "explicit none",           // compiled to TryGetValue(out None)
        null   => "default/empty",           // compiled to HasValue
    };

    public static int SumBoxed(int n)
    {
        int sum = 0;
        for (int i = 0; i < n; i++) { BoxedOption<int> o = i; if (o is int v) sum += v; }
        return sum;
    }

    public static int SumValueOption(int n)
    {
        int sum = 0;
        for (int i = 0; i < n; i++) { ValueOption<int> o = i; if (o is int v) sum += v; }
        return sum;
    }
}
