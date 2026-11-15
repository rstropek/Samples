I answered from memory as you asked and didn't check any documentation. Several answers below are uncertain.

1. `public union Pet(Cat, Dog);`

2. It lowers to a **struct** marked with `[Union]` that implements `System.Runtime.CompilerServices.IUnion`. It has one constructor per case type and an `object? Value` property. Pattern matching and switches look through to `Value`, and switches over all the case types are exhaustive. I'm fairly sure of the overall shape but not sure of every exact name.

3. Not sure it's in C# 15. The proposal I know of is **`closed`** classes (`closed class Shape`). A closed class is implicitly abstract, and only types in the same assembly can derive from it directly. That lets the compiler treat a switch over the known subtypes as exhaustive.

4. Not sure. My guess is that STJ writes the active case's `Value` as that case type. For two object types with no discriminator, deserializing would mean picking a case by shape or with a custom converter. I don't remember the exact .NET 11 API.

5. Not sure. Possibly an option that infers derived types from a closed hierarchy, or some other new knob. I can't name it with confidence.

6. Yes. The compiler-generated union stores the case in `object? Value`, so an `int` gets boxed. A custom `[Union]` type can avoid boxing by storing the cases in typed fields itself. It then exposes the non-boxing access pattern (`HasValue` plus `TryGetValue(out T)` overloads), which the compiler uses for pattern matching instead of `Value`.

7. The declaration compiles, but the two case constructors and conversions become the same `string` signature. Creating a value or implicitly converting a string is then ambiguous. At runtime there's no tag, so the union can't tell whether a string is the "T0" or the "T1" case. I'm fairly but not fully sure of the exact diagnostics.

8. Not sure. Unions arrived during the .NET 11 previews, and I don't know whether they, `closed`, or the STJ support ship as stable or stay in preview in GA.
