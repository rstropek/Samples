// ============================================================================
//  00_Rust — the C# 15 union demos, written in Rust.
//
//  WHY THIS SAMPLE: Rust has had discriminated unions ("sum types") since day
//  one, spelled `enum`, and ships `Option<T>` / `Result<T, E>` in its standard
//  library. C# 15 unions bring the same modelling power to C#. Run this next to
//  `01_Basics` and `02_ResultOption` to show how close the two now are.
//
//  C# 15                                  Rust
//  -------------------------------------  ----------------------------------
//  union Pet(Cat, Dog, Bird);             enum Pet { Cat{..}, Dog{..}, Bird{..} }
//  Pet p = new Dog("Rex");                let p = Pet::Dog { name: "Rex".into() };
//  pet switch { ... }   (exhaustive)      match pet { ... }   (exhaustive, an ERROR if not)
//  union Option<T>(Some<T>, None);        std Option<T> { Some(T), None }
//  union Result<T,E>(Ok<T>, Error<E>);    std Result<T, E> { Ok(T), Err(E) }
// ============================================================================

// ----------------------------------------------------------------------------
//  Part 1 — mirrors 01_Basics: a `Pet` sum type
//  In Rust the discriminated union IS the `enum`; each variant carries its own
//  data. No attribute, no `object?` box — the tag is part of the type.
// ----------------------------------------------------------------------------
#[derive(Debug)]
enum Pet {
    Cat { name: String },
    Dog { name: String },
    Bird { name: String, can_fly: bool },
}

impl Pet {
    // Exhaustive match. Unlike C#'s CS8509 *warning*, Rust makes a non-exhaustive
    // match a hard compile ERROR — and there is no empty/`default` case to leak,
    // because an enum value is always exactly one variant.
    fn describe(&self) -> String {
        match self {
            Pet::Cat { name } => format!("Cat named {name}"),
            Pet::Dog { name } => format!("Dog named {name}"),
            Pet::Bird { name, can_fly: true } => format!("Bird named {name} (flies)"),
            Pet::Bird { name, can_fly: false } => format!("Bird named {name} (grounded)"),
        }
    }
}

fn pet_basics() {
    println!("== Part 1: Pet enum (mirrors 01_Basics) ==");

    // 1. Construction. Note the difference from C#: Rust has NO implicit
    //    conversion from a variant's "type" — you name the variant explicitly.
    let a = Pet::Dog { name: "Rex".into() };
    let b = Pet::Cat { name: "Whiskers".into() };
    let c = Pet::Bird { name: "Tweety".into(), can_fly: true };

    // 2. Exhaustive match.
    println!("{}", a.describe());
    println!("{}", b.describe());
    println!("{}", c.describe());

    // 3. Enums are ordinary values: put them in a Vec and count the dogs.
    let shelter = vec![a, b, c, Pet::Dog { name: "Buddy".into() }];
    let dogs = shelter.iter().filter(|p| matches!(p, Pet::Dog { .. })).count();
    println!("\nShelter holds {} pets, {} of them dogs.", shelter.len(), dogs);

    // 4. Pattern matching reaches into the variant's fields (`if let`).
    for pet in &shelter {
        if let Pet::Bird { name, can_fly: true } = pet {
            println!("{name} can fly away!");
        }
    }

    // 5. No `object? Value` escape hatch is needed: Rust enums are not a boxed
    //    wrapper, so there is no "underlying object" to reach for — the variant
    //    *is* the value.
}

// ----------------------------------------------------------------------------
//  Part 2 — mirrors 02_ResultOption: Option<T> and Result<T, E>
//  Both are in Rust's std library; C# 15 lets you declare equivalents as unions.
// ----------------------------------------------------------------------------

// Option<T>: "a value, or nothing".
fn half(x: i32) -> Option<i32> {
    if x % 2 == 0 { Some(x / 2) } else { None }
}

// Result<T, E>: "success T, or failure E".
fn parse(s: &str) -> Result<i32, String> {
    s.parse::<i32>().map_err(|_| format!("'{s}' is not an integer"))
}

// The `?` operator: short-circuit on the first Err. C# has no direct equivalent
// yet — with unions you'd write a Bind/Match chain (see 02_ResultOption).
fn add_parsed(a: &str, b: &str) -> Result<i32, String> {
    Ok(parse(a)? + parse(b)?)
}

fn option_result_basics() {
    println!("\n== Part 2: Option<T> / Result<T,E> (mirrors 02_ResultOption) ==");

    // Option: map (Map), and_then (Bind), unwrap_or (GetValueOrDefault), match.
    let mapped = Some(3).map(|x| x * 2).unwrap_or(0);          // 6
    let chained = Some(8).and_then(half).unwrap_or(-1);        // 4
    let empty = None::<i32>.map(|x| x * 2).unwrap_or(0);       // 0
    println!("Option: map={mapped}, and_then={chained}, none={empty}");

    let rendered = match Some(5) {
        Some(v) => format!("some {v}"),
        None => "none".to_string(),
    };
    println!("Option match: {rendered}");

    // Result: map, match, and the `?` operator.
    let ok = parse("21").map(|x| x * 2);                      // Ok(42)
    let bad = parse("oops").map(|x| x * 2);                   // Err(...)
    println!("Result ok:  {}", render(&ok));
    println!("Result err: {}", render(&bad));

    println!("add_parsed(\"40\",\"2\") -> {}", render(&add_parsed("40", "2")));
    println!("add_parsed(\"40\",\"x\") -> {}", render(&add_parsed("40", "x")));
}

fn render(r: &Result<i32, String>) -> String {
    match r {
        Ok(v) => format!("ok={v}"),
        Err(e) => format!("err={e}"),
    }
}

fn main() {
    pet_basics();
    option_result_basics();
}
