# C# 14 – First-Class **Span** Conversions

`System.Span<T>` and `System.ReadOnlySpan<T>` are lightweight, stack-only types that enable high-performance, allocation-free access to contiguous memory.  
Up to C# 13 the language recognised them, but certain conversions still required explicit casts.  

C# 14 adds a **family of implicit conversions** that make span programming feel as natural as working with arrays or strings.

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

## Walking through the sample (`Program.cs`)

[`Program.cs`](./Program.cs) exercises each new conversion.

## Gotchas

* There is **no implicit `ReadOnlySpan<T>` → `Span<T>`**. Mutability never flows implicitly.
* Multidimensional or jagged arrays are **not** convertible; only `T[]`.
