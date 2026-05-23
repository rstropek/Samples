# C# 14 Null-Conditional **Assignment**

C# has supported *null-conditional member access* (`?.` / `?[]`) for quite a while. Until now those operators were **read-only**: you could *observe* a member safely, but you could not **assign** to it.  

C# 14 finally lifts that limitation – the null-conditional operator can appear on the **left-hand side of an assignment or compound assignment**.

## Why is this useful?

* **Fewer null checks** – no need for an `if (obj is not null)` wrapper.
* **Short-circuit evaluation** – the right side is only evaluated when the receiver chain isn't `null`.
* **Works with `+=`, `-=`, `*=`, …** – but **not** with `++` / `--`.

## Sample walk-through (`Program.cs`)

The code in [`Program.cs`](./Program.cs) demonstrates two scenarios:

1. **Updating a nested property**: *If* `p` **and** `p.Address` are non-null, the street is upper-cased; otherwise nothing happens and `ToUpper()` is **not** executed.
2. **Using a null-conditional indexer**: Safe even though the array reference is `null`.

## Things to remember

* Increment/decrement operators (`++`, `--`) are **excluded** because they need to read and then write – the intermediate value would be undefined when the object is null.
* The feature also works with **compound assignment** (`?.Count += 1`) following the same rules.
* There's *no* hidden reflection or dynamic dispatch – just a couple of null checks.
