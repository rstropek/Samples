# C# 14 Lambda Parameter Modifiers on Simple Lambdas

C# 14 removes a long-standing restriction that forced you to **specify the parameter type** whenever you wanted to use a modifier such as `ref`, `in`, `out`, `scoped`, or `ref readonly` on a lambda parameter.

With this change you can now write concise, type-inferred lambda expressions that still leverage the power of parameter modifiers.

## Why it matters

* **Less repetition** – you no longer need to repeat obvious types just to introduce a modifier.
* **Cleaner delegate declarations** – lambdas often appear in LINQ queries, callbacks, or event subscriptions where brevity is key.

Both versions compile to identical IL – the new syntax just saves keystrokes.

The only modifier that **still requires an explicit type list** is `params`.

## Walk-through of the sample (`Program.cs`)

Open [`Program.cs`](./Program.cs) to see the feature in action.

1. **Legacy version** – `tryParseCsv` with explicit types to satisfy the old compiler.
2. **Modern version** – new syntax: the compiler infers `string` and `List<int>` from the delegate `TryParseCsv` while still honouring the `out` modifier.
3. Both delegates are invoked in exactly the same way; the behaviour is unchanged.
