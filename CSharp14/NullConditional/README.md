# C# 14 Null-Conditional **Assignment**

C# has supported *null-conditional member access* (`?.` / `?[]`) since version 6. Until now those operators were **read-only**: you could *observe* a member safely, but you could not **assign** to it.  

C# 14 finally lifts that limitation – the null-conditional operator can appear on the **left-hand side of an assignment or compound assignment**.

> References: "What's new in C# 14" (Microsoft Learn) [[link](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)]

---

## Why is this useful?

* **Fewer null checks** – no need for an `if (obj is not null)` wrapper.
* **Short-circuit evaluation** – the right side is only evaluated when the receiver chain isn't `null`.
* **Works with `+=`, `-=`, `*=`, …** – but **not** with `++` / `--`.

---

## Sample walk-through (`Program.cs`)

The code in [`Program.cs`](./Program.cs) demonstrates two scenarios:

1. **Updating a nested property**
   ```csharp
   p?.Address?.Street = p.Address.Street.ToUpper();
   ```
   *If* `p` **and** `p.Address` are non-null, the street is upper-cased; otherwise nothing happens and `ToUpper()` is **not** executed.

2. **Using a null-conditional indexer**
   ```csharp
   Person[]? ps = null;
   ps?[0].Name = "John Doe";
   ```
   Safe even though the array reference is `null`.

Run it:
```bash
cd CSharp14/NullConditional
dotnet run
```
You'll see that no exceptions are thrown and the output shows `""` for assignments that were skipped.

---

## Things to remember

* Only the **member access expression** (`obj?.Prop`) can be null; any earlier part of the chain may still throw if it's null.
* Increment/decrement operators (`++`, `--`) are **excluded** because they need to read and then write – the intermediate value would be undefined when the object is null.
* The feature also works with **compound assignment** (`?.Count += 1`) following the same rules.

---

## Performance note

The compiler emits code equivalent to:
```csharp
if (p is not null && p.Address is not null)
{
    p.Address.Street = p.Address.Street.ToUpper();
}
```
So there's *no* hidden reflection or dynamic dispatch – just a couple of null checks.

---

## Further reading

* Microsoft Docs – What's new in C# 14 [[link](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)]
* Syncfusion Blog – Key features in C# 14 [[link](https://www.syncfusion.com/blogs/post/whats-new-in-csharp-14-key-features)]