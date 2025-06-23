# C# 14 – First-Class **Span** Conversions

`System.Span<T>` and `System.ReadOnlySpan<T>` are lightweight, stack-only types that enable high-performance, allocation-free access to contiguous memory.  
Up to C# 13 the language recognised them, but certain conversions still required explicit casts.  

C# 14 adds a **family of implicit conversions** that make span programming feel as natural as working with arrays or strings.

> Docs: Microsoft Learn – *What's new in C# 14* [[link](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-14)]

---

## New implicit conversions

1. **T[] → Span<T> / ReadOnlySpan<T>**  
   Any one-dimensional managed array now flows seamlessly into span variables.
2. **Span<T> → ReadOnlySpan<T>**  
   Moving from a mutable to a read-only view no longer needs `.AsReadOnly()`.
3. **Covariant spans**  
   `Span<Derived>` and `ReadOnlySpan<Derived>` convert to `ReadOnlySpan<Base>` when normal reference covariance applies.
4. **string → ReadOnlySpan<char>**  
   Useful for high-performance parsing without `Substring` allocations.

All conversions are **alloc-free** – they simply reinterpret the underlying memory.

---

## Walking through the sample (`Program.cs`)

[`Program.cs`](./Program.cs) exercises each new conversion. Highlights:

```csharp
Span<int> s1 = new int[10];              // array → Span<int>
ReadOnlySpan<int> rs1 = new int[10];      // array → ReadOnlySpan<int>

Span<Person> people = new Person[10];     // array of reference-types is fine
ReadOnlySpan<Person> ro = people;         // Span<T> → ReadOnlySpan<T>

ReadOnlySpan<char> chars = "Hello, World!"; // string → ReadOnlySpan<char>
```

Compile & run:
```bash
cd CSharp14/SpanConversions
dotnet run
```
No output – the program is compile-time verification only. Remove `#pragma` and add your own logic to see run-time effects.

---

## Design rationale

* **Safety** – spans remain confined to the stack and respect lifetime checks; only conversions that preserve safety are allowed.
* **Performance** – by eliminating helper calls you reduce JIT overhead and enable further optimisations.
* **Generic friendliness** – the compiler now considers these conversions during type inference, making generic libraries easier to write.

---

## Gotchas

* There is **no implicit `ReadOnlySpan<T>` → `Span<T>`**. Mutability never flows implicitly.
* Multidimensional or jagged arrays are **not** convertible; only `T[]`.
* Remember that a span's lifetime is bounded by the array/string lifetime plus the current stack frame.

---

## Further reading

* Span usage guidelines – .NET Performance docs [[link](https://learn.microsoft.com/dotnet/standard/performance/span-usage-guidelines)]
* Syncfusion blog – Key features coming in C# 14 [[link](https://www.syncfusion.com/blogs/post/whats-new-in-csharp-14-key-features)]