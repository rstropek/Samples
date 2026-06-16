# 00_Rust — the same demos in Rust

A Rust counterpart to `01_Basics` and the `Option`/`Result` basics from
`02_ResultOption`. The point of having it: Rust has had discriminated unions
(`enum`) and `Option`/`Result` from the start, so showing them side by side
makes it obvious what C# 15 unions are catching up to.

This is a Cargo project, **not** part of `CSharp15.slnx` (that's a .NET
solution). Run it on its own:

```bash
cd src/00_Rust
cargo run        # needs a Rust toolchain (https://rustup.rs)
```

## C# 15 ↔ Rust mapping

- **Declaration** — C#: `union Pet(Cat, Dog, Bird);` · Rust: `enum Pet { Cat{..}, Dog{..}, Bird{..} }`. In Rust the discriminated union *is* the `enum`; each variant carries its own data.
- **Construction** — C#: `Pet p = new Dog("Rex");` (implicit conversion from the case type) · Rust: `let p = Pet::Dog { name: "Rex".into() };` (you name the variant; no implicit conversion).
- **Exhaustive match** — C#: `pet switch { ... }` warns (CS8509) if a case is missing · Rust: `match pet { ... }` is a hard compile **error** if a variant is missing. Rust also has no empty/`default` value to leak — an `enum` is always exactly one variant (C#'s union is a struct, so `default` exists).
- **Option** — C#: `union Option<T>(Some<T>, None);` (you declare it) · Rust: `Option<T> { Some(T), None }` in std. Combinators line up: `Map`↔`map`, `Bind`↔`and_then`, `GetValueOrDefault`↔`unwrap_or`, `Match`↔`match`.
- **Result** — C#: `union Result<T,E>(Ok<T>, Error<E>);` · Rust: `Result<T, E> { Ok(T), Err(E) }` in std, plus the `?` operator for early-return on `Err` (C# has no direct equivalent yet — you'd chain `Bind`/`Match`).

## Output

```text
== Part 1: Pet enum (mirrors 01_Basics) ==
Dog named Rex
Cat named Whiskers
Bird named Tweety (flies)

Shelter holds 4 pets, 2 of them dogs.
Tweety can fly away!

== Part 2: Option<T> / Result<T,E> (mirrors 02_ResultOption) ==
Option: map=6, and_then=4, none=0
Option match: some 5
Result ok:  ok=42
Result err: err='oops' is not an integer
add_parsed("40","2") -> ok=42
add_parsed("40","x") -> err='x' is not an integer
```
