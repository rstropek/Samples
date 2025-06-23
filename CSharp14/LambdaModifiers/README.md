# C# 14 Lambda Parameter Modifiers on Simple Lambdas

C# 14 removes a long-standing restriction that forced you to **specify the parameter type** whenever you wanted to use a modifier such as `ref`, `in`, `out`, `scoped`, or `ref readonly` on a lambda parameter.

With this change you can now write concise, type-inferred lambda expressions that still leverage the power of parameter modifiers.

> 📚 Background information taken from the official “What's new in C# 14” article on Microsoft Learn and other preview announcements. [[link](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)]

---

## Why it matters

* **Less repetition** – you no longer need to repeat obvious types just to introduce a modifier.
* **Cleaner delegate declarations** – lambdas often appear in LINQ queries, callbacks, or event subscriptions where brevity is key.
* **Full parity with method syntax** – ordinary methods have supported these modifiers since C# 7; lambdas finally catch up.

| Before C# 14 | After C# 14 |
|--------------|-------------|
|```csharp
TryParseCsv parser = (string input, out List<int> result) => ...;
```|```csharp
TryParseCsv parser = (input, out result) => ...;
```|

Both versions compile to identical IL – the new syntax just saves keystrokes.

> ❗ The only modifier that **still requires an explicit type list** is `params`.

---

## Walk-through of the sample (`Program.cs`)

Open [`Program.cs`](./Program.cs) to see the feature in action.

1. **Legacy version** – Lines 1-15 declare `tryParseCsv` with explicit types to satisfy the old compiler.
2. **Modern version** – Lines 18-30 show the new syntax: the compiler infers `string` and `List<int>` from the delegate `TryParseCsv` while still honouring the `out` modifier.
3. Both delegates are invoked in exactly the same way; the behaviour is unchanged.

Compile & run:
```bash
cd CSharp14/LambdaModifiers
dotnet run
```
You'll see identical results confirming functional parity.

---

## Design notes

* **Type inference** is possible because a *simple-lambda*'s parameter list is matched against the expected delegate type.
* The **modifier tokens become part of the lambda signature**; the generated method retains `ref`, `out`, etc., semantics.
* This change applies to both **expression-bodied** and **statement-bodied** lambdas.

---

## When to use it

* Parsing or conversion delegates that naturally use `out` parameters (e.g., `int.TryParse`).
* Performance-critical callbacks that pass large structs by `in`/`ref` without cluttering the call-site with types.
* Any existing code where the parameter types were only there to allow a modifier – you can now simplify it.

---

## Further reading

* Microsoft Docs – What's new in C# 14 [[link](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)]
* Syncfusion blog – Upcoming C# 14 features [[link](https://www.syncfusion.com/blogs/post/whats-new-in-csharp-14-key-features)]

Enjoy the cleaner syntax! 🎉