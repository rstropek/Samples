# C# 14 – **User-Defined Compound Assignment Operators**

Historically, operator overloading in C# required **static** methods.  
Defining the binary `+` operator automatically gave you `+=` for free – but that came at a cost: the compiler had to allocate a **new instance** for every compound assignment (`a = a + b`).

C# 14 changes the game by allowing you to **explicitly implement the compound operator itself** (e.g. `operator +=`) as an **instance** method with a `void` return type.  
The result is *in-place mutation* with zero allocations – perfect for performance-sensitive structs and large classes.

## The sample (`Program.cs`)

[`Program.cs`](./Program.cs) contains two `Vector3D` types:

* **`Vector3D`** – classic, allocates on every `+=` because it only has a static `+` operator.
* **`Vector3DNew`** – defines `void operator +=(Vector3DNew rhs)` and mutates the coordinates directly.

## Rules for compound operator overloads

1. **Instance, not static** – the whole point is to mutate the existing instance.
2. **Return type must be `void`.**
3. Exactly **one parameter** representing the right-hand side.
4. Operator name must match the compound symbol (`+=`, `-=`, `*=`, `/=`, `^=`, etc.).
5. Increment/decrement (`++`, `--`) can also be instance methods with zero parameters in C# 14.

The compiler prefers the instance overload whenever the left-hand side is a variable reference. If none exists it falls back to the generated static pattern, preserving backward compatibility.

## Design benefits

* **Performance** – avoids unnecessary allocations and GC pressure.
* **Expressiveness** – lets your type expose *true* mutating semantics when appropriate (vectors, matrices, immutable-by-default record builders, …).
* **Symmetry** – you can still provide the regular binary operator for non-mutating scenarios.

## Caveats

You are responsible for keeping `+=` **semantically consistent** with `x = x + y`. Surprising behaviour is considered bad practice.
