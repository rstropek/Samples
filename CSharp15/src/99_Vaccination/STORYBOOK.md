# Storybook: the vaccination center

A presenter script for building `src/99_Vaccination` live during the full-day C# 15 workshop.
The sample is built in five steps. Each step follows the same rhythm:

1. **Discuss first:** the small samples that introduce the features.
2. **Model it first:** show the requirements and let the group design the types before any code
   is written. Each step lists the answers you'll likely hear (often the C# 14 way), how to respond,
   and where we land.
3. **Code and run:** build the step and compare the result with the group's design.
4. **Break it on purpose:** small experiments that show what each feature protects against.

The day ends with a modeling exercise without code (the vaccination day itself).

## The story

It's flu and COVID-19 season. A city runs a vaccination center and its online portal drops one JSON
file per booking request into a folder. Our console app:

1. reads the requests (one person or a whole family, in a format we don't control),
2. checks every requested vaccine against the person's age and vaccination history,
3. books back-to-back slots (a family is vaccinated one after another in the same booth),
4. stores all appointments in a JSON file and prints the day plan.

Halfway through the season an RSV vaccine is added. The compiler tells us what to update.

The eligibility rules are simplified demo rules. Say so early, so nobody starts discussing real
vaccination recommendations.

## Features and where they appear

| C# 15 / .NET 11 feature | Where in the sample | Step |
|---|---|---|
| Union of existing types | `PortalRequest(SingleRequest, FamilyRequest)` | 1 |
| Members in a union body | `PortalRequest.Email` and `.Members` switch over `this` | 1 |
| JSON: union with structural classifier | The portal format has no type field, so the case is picked by property names | 1 |
| JSON: strict deserialization | `RespectRequiredConstructorParameters`, `RespectNullableAnnotations` | 1 |
| Closed hierarchy | `closed record Vaccine` with `Influenza`, `Covid` (later `Rsv`) | 2 |
| Exhaustive switch, `when` guards, no `_` arm | `Rules.Check`, `Vaccine.ToString` | 2 |
| Union with a closed hierarchy as a case | `Eligibility(Vaccine, TooSoon, NotRecommended)` | 2 |
| JSON: inferred polymorphism for closed types | `history.json`, `appointments.json` (`$type`) | 2, 4 |
| Union as a result type | `BookingResult(Appointment, Duplicate, NothingToBook, NoFreeSlot)` | 3 |
| Collection expression arguments | `HashSet<string>` with `with(StringComparer.OrdinalIgnoreCase)` | 3 |
| Extension indexer | `appointments[slot]` on a plain `List<Appointment>` | 3 |
| Labeled `continue` | `continue booth;` in the slot search | 3 |
| "Add a case" safety | RSV: two compile errors, no JSON changes | 5 |

**Deliberately left out**, because the domain has no natural place for them:

- **Custom non-boxing unions** (`13_CustomUnions`): there is no hot path here.
- **Runtime async** (`08`/`09`): the app does no meaningful async work.
- **Memory safety** (`16_MemorySafety`): it has its own sample.

Saying this out loud is a teaching point in itself: not every feature belongs in every codebase.

## Preparation

Create an empty project next to the finished sample and copy the demo data:

```bash
cd src
mkdir 99_Live
cp 99_Vaccination/99_Vaccination.csproj 99_Live/
cp -R 99_Vaccination/data 99_Live/
rm -f 99_Live/data/appointments.json
cd 99_Live
```

The project gets `net11.0`, nullable and implicit usings from the repo's `Directory.Build.props`.
Show the `.csproj`: it makes the two "switch is not exhaustive" warnings errors. CS8509 means a case
has no arm at all. CS8846 means a case only has arms with a `when` guard. Mention it now; it pays
off in steps 2 and 5.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <RootNamespace>Vaccination</RootNamespace>
    <!-- A missing case in a switch over a union or a closed hierarchy is a build
         error here, not just a warning. That turns "we added a new vaccine" into
         a to-do list from the compiler.
         CS8509: a case has no arm at all.
         CS8846: a case only has arms with a `when` guard, so some values fall through. -->
    <WarningsAsErrors>$(WarningsAsErrors);CS8509;CS8846</WarningsAsErrors>
  </PropertyGroup>
</Project>
```

Walk through `data/` briefly:

- `inbox/*.json`: six portal requests. 001 is a single person, 002 a family, 003 someone vaccinated
  recently, 004 a duplicate of 001 with different email casing, 005 is missing a birth date, 006
  asks for RSV.
- `history.json`: earlier vaccinations, imported from the e-vaccination pass.
- `appointments.seed.json`: three appointments that already existed before the demo.

Always run the app from the project folder (`dotnet run`), because it reads `data/` relative to the
current directory. The finished sample in `src/99_Vaccination` is the state after step 5 (apart
from its header comment in `Program.cs`). There are no snapshots of the steps in between; the code
blocks below are complete.

---

## Step 1: Read the portal requests

**Discuss first:** `01_Basics`, `11_UnionWithMembers`, and section (2) of `14_JsonUnions`.

**Goal:** read every inbox file into a typed request, whether it's for one person or a family.

### Model it first

Show the requirements and let the group design the C# types. Collect the answers before you
comment on them.

> **Requirements**
>
> - The city's portal sends one JSON file per booking. We can't change its format.
> - A booking is either for one person or for a family:
>
>   ```json
>   { "email": "...", "person":  { "name": "...", "birthDate": "...", ... } }
>   { "email": "...", "members": [ { ... }, { ... } ] }
>   ```
>
> - No field says which of the two it is.
> - The rest of the app only needs an email address and a list of people.
> - Broken files must be rejected, not half-read.

**Ask:**

1. Which C# types would you write for a request?
2. How does the deserializer know which of the two shapes it is reading?
3. Where do the two shapes become "an email and a list of people"?
4. What happens with a file that is missing a field?

**Answers you'll likely hear, and how to respond:**

- *"One class with a nullable `Person` and a nullable `Members`."* Ask what it means when both are
  null, or both are set. The type allows states the portal never sends, and every consumer needs
  null checks.
- *"An abstract base class, two subclasses, and a custom `JsonConverter`."* That's the C# 14 answer
  and it works. Ask who writes and maintains the converter that looks at property names
  (`CSharp14Way.cs` in `14_JsonUnions` has one).
- *"A closed hierarchy with `$type`."* A good instinct for our own data, but the portal's JSON has no
  `$type` and we can't add one.
- *"Treat a single person as a family of one."* That's what the app does internally (`Members`).
  But the JSON still comes in two shapes, so reading it needs both.

**Where we land:** two plain records, one per shape, and `union PortalRequest(SingleRequest,
FamilyRequest)`. They don't share a base class, so this is a union of existing types. The
structural classifier picks the case by its property names. Members in the union body turn both
shapes into `Email` and `Members`, so nothing else in the app switches over the shape. Strict JSON
options reject files with missing fields.

### Code

`PortalRequest.cs`:

```csharp
using System.Text.Json.Serialization;

namespace Vaccination;

// The city's booking portal drops one JSON file per submission. We don't own
// that format, and it has no type field: a submission is either for one person
//   { "email": "...", "person":  { ... } }
// or for a family
//   { "email": "...", "members": [ { ... }, { ... } ] }
// The structural classifier picks the case by its property names.
public sealed record PortalPerson(string Name, DateOnly BirthDate, string InsuranceNumber, string[] Vaccines, bool RiskFactors = false);

public sealed record SingleRequest(string Email, PortalPerson Person);
public sealed record FamilyRequest(string Email, PortalPerson[] Members);

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
public union PortalRequest(SingleRequest, FamilyRequest)
{
    // The rest of the app doesn't care which shape the portal sent.
    public string Email => this switch
    {
        SingleRequest s => s.Email,
        FamilyRequest f => f.Email,
    };

    public PortalPerson[] Members => this switch
    {
        SingleRequest s => [s.Person],
        FamilyRequest f => f.Members,
    };
}
```

Optional live-coding detour: write the union without a body first and put the switch into
`Program.cs`, for example `string email = request switch { SingleRequest s => s.Email,
FamilyRequest f => f.Email };`. Then move it into the union body, as shown above, to show that a
union can have members. Inside the body, switch over `this`, not `Value`: `Value` is `object?`, so a
switch over it isn't exhaustive. The `Program.cs` below already uses the members.

`Storage.cs`:

```csharp
using System.Text.Json;

namespace Vaccination;

public static class Storage
{
    // camelCase like the portal, and strict: a submission with a missing or
    // null required field is rejected instead of turning into half a record.
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        RespectRequiredConstructorParameters = true,
        RespectNullableAnnotations = true,
    };

    public static T Read<T>(string path) =>
        JsonSerializer.Deserialize<T>(File.ReadAllText(path), Options)
        ?? throw new JsonException($"{path} is empty");

    public static void Write<T>(string path, T value) =>
        File.WriteAllText(path, JsonSerializer.Serialize(value, Options));
}
```

`Program.cs`:

```csharp
using System.Text.Json;
using Vaccination;

foreach (var file in Directory.GetFiles(Path.Combine("data", "inbox"), "*.json").Order())
{
    Console.WriteLine(Path.GetFileName(file));

    PortalRequest request;
    try { request = Storage.Read<PortalRequest>(file); }
    catch (JsonException ex)
    {
        Console.WriteLine($"  rejected: {ex.Message}");
        Console.WriteLine();
        continue;
    }

    Console.WriteLine($"  from {request.Email}");
    foreach (var member in request.Members)
        Console.WriteLine($"  {member.Name,-16} born {member.BirthDate:d MMM yyyy}, wants {string.Join(", ", member.Vaccines)}");
    Console.WriteLine();
}
```

**Run** (`dotnet run`). Excerpt:

```text
001-berger.json
  from anna.berger@example.at
  Anna Berger      born 12 Mar 1958, wants flu, covid

002-huber.json
  from huber.family@example.at
  Thomas Huber     born 2 Jul 1984, wants flu, covid
  Sabine Huber     born 30 Nov 1986, wants flu, covid
  Lena Huber       born 21 May 2014, wants flu
...
005-novak.json
  rejected: JSON deserialization for type 'Vaccination.PortalPerson' was missing required properties including: 'birthDate'.
```

**Break it on purpose:**

1. Remove the `[JsonUnion(...)]` attribute. Every file is rejected at runtime:
   `JSON value type 'Object' is ambiguous for union type 'Vaccination.PortalRequest' because
   multiple case types can use this value type.` Two object cases need the structural classifier.
2. Put the attribute back. Remove the two `Respect...` lines in `Storage.cs`. Request 005 is now
   accepted, and Peter Novak is "born 1 Jan 0001". Teaser for step 2: once the eligibility rules
   exist, he would count as 2025 years old and get a high-dose flu shot. Strict options are the
   cheap version of input validation at a boundary you don't control. Put the lines back.

---

## Step 2: Vaccines and eligibility

**Discuss first:** `12_ClosedHierarchies`, `04_DomainModeling`, `05_PatternShowcase`, and
section (3) of `14_JsonUnions`.

**Goal:** for each requested vaccine, decide whether the person gets it, and if not, why not.

### Model it first

> **Requirements** (simplified demo rules)
>
> - The center offers flu and COVID-19 vaccination. More vaccines may follow.
> - People of 60 and over get the high-dose flu vaccine.
> - Flu: once per season. The season starts on 1 September.
> - COVID-19: only from 60 or with risk factors, and at most once in 12 months.
> - Nobody under 18. Children go to their paediatrician.
> - When the answer is no, the person needs to know why. If it's "not yet", they need the earliest
>   date.

**Ask:**

1. How do we represent the vaccines?
2. What does `Check(person, vaccine)` return?

**Answers you'll likely hear for the vaccines:**

- *"An enum `VaccineKind { Flu, Covid }`."* Two problems:
  - Where does `HighDose` go? A `bool HighDose` next to the enum is meaningless for COVID-19.
  - A switch over an enum is never exhaustive. Even with every named value handled, the compiler
    warns with CS8524, because an enum can hold any `int`. Teams then add `_ => throw ...`, which
    hides the next new vaccine from the compiler.
- *"A base class with subclasses."* Close. But without `closed` the compiler can't know that the
  list is complete, so every switch still needs a `_` arm.
- *"A union of records."* Possible, but we own all the types, they share behavior (`ToString`),
  and in step 4 we'll store them with a type discriminator. That is what closed hierarchies are
  for.

**Answers you'll likely hear for the result:**

- *"`bool`."* Loses the reason and the date.
- *"`(bool Ok, string? Reason, DateOnly? EarliestDate)`", or a class with nullable fields.* Ask
  which combinations are valid. `Ok = true` with a reason? The type doesn't say.
- *"Throw an exception."* "Not eligible" is a normal answer, not an error. And the signature
  doesn't tell the caller which exceptions to expect.
- *"`Result<Vaccine, string>`."* Better. But "not yet" carries a date and "not recommended" a text.
  Squeezing both into `string` loses the date.

**Where we land:**

- **`closed record Vaccine`** with `Influenza(bool HighDose)` and `Covid`. Each case carries only
  its own data, and switches are exhaustive without `_`.
- **`union Eligibility(Vaccine, TooSoon, NotRecommended)`.** "Yes" is answered with the vaccine to
  give: the rules turn a flu request into a high-dose one for people over 60. `Vaccine` is an
  existing type and a closed hierarchy itself, so it can't also derive from a result base class.
  That's where a union fits better than a second closed hierarchy.

### Code

`Domain.cs` (first version):

```csharp
using System.Text.Json.Serialization;

namespace Vaccination;

public sealed record Person(string InsuranceNumber, string Name, DateOnly BirthDate, bool RiskFactors)
{
    public int AgeOn(DateOnly day)
    {
        int age = day.Year - BirthDate.Year;
        return BirthDate > day.AddYears(-age) ? age - 1 : age;
    }
}

// The vaccines this center offers. We own every case and want the compiler to
// know the complete list, so this is a closed hierarchy, not a union.
// `closed` also tells System.Text.Json the complete list: every vaccine gets a
// "$type" discriminator without a [JsonDerivedType] for each subtype.
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record Vaccine
{
    // Sealed, so the derived records don't generate their own ToString().
    public sealed override string ToString() => this switch
    {
        Influenza { HighDose: true } => "Influenza (high dose)",
        Influenza => "Influenza",
        Covid => "COVID-19",
    };
}

public sealed record Influenza(bool HighDose) : Vaccine;
public sealed record Covid : Vaccine;

// One entry in a person's vaccination record (imported from the e-vaccination pass).
public sealed record VaccinationRecord(string InsuranceNumber, Vaccine Vaccine, DateOnly Date);
```

`Eligibility.cs`:

```csharp
namespace Vaccination;

public sealed record TooSoon(DateOnly EarliestDate);
public sealed record NotRecommended(string Reason);

// The answer to "can this person get this vaccine?". Each case carries only
// the data that makes sense for it. `Vaccine` is an existing type (and a closed
// hierarchy itself), so it can't derive from a common "result" base: this is
// where a union fits better than a closed hierarchy.
public union Eligibility(Vaccine, TooSoon, NotRecommended);

// Simplified DEMO rules. They are not official vaccination recommendations.
public static class Rules
{
    public static Eligibility Check(Person person, string requested, IEnumerable<VaccinationRecord> history, DateOnly today)
    {
        Vaccine? wanted = requested.ToLowerInvariant() switch
        {
            "flu" => new Influenza(HighDose: false),
            "covid" => new Covid(),
            _ => null,
        };
        if (wanted is null) return new NotRecommended($"'{requested}' is not offered here");

        int age = person.AgeOn(today);
        if (age < 18) return new NotRecommended("under 18, please see your paediatrician");

        var past = history.Where(r => r.InsuranceNumber == person.InsuranceNumber).ToList();
        DateOnly seasonStart = new(today.Month >= 9 ? today.Year : today.Year - 1, 9, 1);

        return wanted switch
        {
            Influenza when LastDose<Influenza>(past) >= seasonStart
                => new TooSoon(seasonStart.AddYears(1)),
            Influenza
                => new Influenza(HighDose: age >= 60),

            Covid when age < 60 && !person.RiskFactors
                => new NotRecommended("COVID-19 under 60 without risk factors: ask your doctor"),
            Covid when LastDose<Covid>(past) is { } last && last.AddYears(1) > today
                => new TooSoon(last.AddYears(1)),
            Covid c
                => c,
        };
    }

    private static DateOnly? LastDose<T>(IEnumerable<VaccinationRecord> past) where T : Vaccine =>
        past.Where(r => r.Vaccine is T).Max(r => (DateOnly?)r.Date);
}
```

Point out:

- Every `when` arm is followed by an unguarded arm for the same type (`Influenza => ...`). Guarded
  arms don't count for exhaustiveness. Without the unguarded arm you get CS8846 ("a pattern with a
  'when' clause might successfully match this value"), an error in this project. Its example may
  name a different type than the one you removed: the compiler shows the first gap it finds.
- `Influenza => new Influenza(HighDose: age >= 60)` converts implicitly to `Eligibility`. No
  factory method and no wrapper are needed.
- `Vaccine.ToString()` is a `sealed override` in the closed base record. Without `sealed`, every
  derived record would generate its own `ToString()` and ignore ours. Because it switches over
  `this`, it takes part in exhaustiveness checking (see step 5).

Add `ToPerson` to `PortalPerson` in `PortalRequest.cs`:

```csharp
public sealed record PortalPerson(string Name, DateOnly BirthDate, string InsuranceNumber, string[] Vaccines, bool RiskFactors = false)
{
    public Person ToPerson() => new(InsuranceNumber, Name, BirthDate, RiskFactors);
}
```

`Program.cs`:

```csharp
using System.Text.Json;
using Vaccination;

// A fixed "today" (the workshop day), so the output is the same on every run.
DateOnly today = new(2026, 11, 24);

var history = Storage.Read<List<VaccinationRecord>>(Path.Combine("data", "history.json"));

foreach (var file in Directory.GetFiles(Path.Combine("data", "inbox"), "*.json").Order())
{
    Console.WriteLine(Path.GetFileName(file));

    PortalRequest request;
    try { request = Storage.Read<PortalRequest>(file); }
    catch (JsonException ex)
    {
        Console.WriteLine($"  rejected: {ex.Message}");
        Console.WriteLine();
        continue;
    }

    foreach (var member in request.Members)
    {
        var person = member.ToPerson();
        foreach (var requested in member.Vaccines)
        {
            Eligibility decision = Rules.Check(person, requested, history, today);
            Console.WriteLine($"  {person.Name,-16} {requested,-6} {decision switch
            {
                Vaccine v => $"yes: {v}",
                TooSoon t => $"not yet, earliest {t.EarliestDate:d MMM yyyy}",
                NotRecommended n => $"no: {n.Reason}",
            }}");
        }
    }
    Console.WriteLine();
}
```

**Run.** Excerpt. The Huber family shows all three cases of the union:

```text
002-huber.json
  Thomas Huber     flu    yes: Influenza
  Thomas Huber     covid  no: COVID-19 under 60 without risk factors: ask your doctor
  Sabine Huber     flu    yes: Influenza
  Sabine Huber     covid  not yet, earliest 1 Mar 2027
  Lena Huber       flu    no: under 18, please see your paediatrician
...
006-steiner.json
  Maria Steiner    rsv    no: 'rsv' is not offered here
  Maria Steiner    flu    yes: Influenza (high dose)
```

**Break it on purpose:**

1. Delete all three `Covid` arms in `Rules.Check`. The build fails with CS8509 ("the pattern
   'Vaccination.Covid' is not covered"). Put them back.
2. Remove `[JsonPolymorphic(InferClosedTypePolymorphism = true)]`. Reading `history.json` now
   fails with `NotSupportedException: Deserialization of interface or abstract types is not
   supported. Type 'Vaccination.Vaccine'.` A closed type is implicitly abstract. Without inference,
   System.Text.Json doesn't know its subtypes. Put the attribute back.

---

## Step 3: Book the slots

**Discuss first:** `07_CollectionExprArgs` and `15_IndexersAndLabels`. `BookingResult` needs
nothing new: it's the same kind of union as `Eligibility` in step 2.

**Goal:** book everyone who got at least one "yes" into back-to-back slots, and print the day plan.

### Model it first

> **Requirements**
>
> - Slots last 15 minutes, from 08:00 to 10:00, in two booths. Bookings are for the next three days.
> - A family is vaccinated one after another in the same booth, so it needs back-to-back free slots.
> - Book the earliest possible time.
> - People press "submit" twice, sometimes with different email casing. Book each email only once.
> - Booking can fail for several reasons, and each one gets its own message.
> - The appointments are a `List<Appointment>`, because that's what we'll store as JSON in step 4.

**Ask:**

1. What does `Book(...)` return?
2. How do we check whether a slot is taken?
3. How do we search for a block of free slots?
4. How do we detect duplicate emails?

**Answers you'll likely hear, and how to respond:**

- **Return type:**
  - *`Appointment?`*: null means it failed, but why?
  - *`bool TryBook(..., out Appointment? appointment, out string? reason)`*: string reasons, and
    every caller has to remember to check them.
  - *Exceptions*: the same problem as in step 2.

  Then ask: what happens to all callers when a new failure reason is added? With a union and an
  exhaustive switch, the compiler lists them.
- **Slot lookup:** a `Schedule` class that wraps the list (more code, and JSON then has to store the
  wrapper), or the same LINQ query in several places. An extension indexer gives the plain list an
  `appointments[slot]` lookup without a wrapper.
- **Search:** nested loops with a `found` flag, or a helper method that returns `bool`. Both work.
  A labeled `continue booth;` says exactly what we mean: this booth doesn't work, try the next.
- **Duplicates:** `ToLowerInvariant()` before every lookup, which is easy to forget in one place, or
  a set that compares case-insensitively from the start.

**Where we land:** `union BookingResult(Appointment, Duplicate, NothingToBook, NoFreeSlot)`, an
extension indexer `this[Slot]` on `IEnumerable<Appointment>`, `continue booth;`, and a
`HashSet<string>` created with `with(StringComparer.OrdinalIgnoreCase)`.

### Code

Add to `Domain.cs`:

```csharp
// A family is vaccinated one after another in the same booth: an appointment
// starts at `Start` and takes one slot per patient.
public readonly record struct Slot(DateOnly Date, TimeOnly Time, int Booth);
public sealed record Patient(Person Person, Vaccine[] Vaccines);
public sealed record Appointment(string Email, Slot Start, Patient[] Patients);
```

`Booking.cs`:

```csharp
namespace Vaccination;

public sealed record Duplicate;
public sealed record NothingToBook;
public sealed record NoFreeSlot(int SlotsNeeded);

// Booking either produces an Appointment (an ordinary domain type that we also
// store in appointments.json) or one of the reasons why it didn't.
public union BookingResult(Appointment, Duplicate, NothingToBook, NoFreeSlot);

public static class Center
{
    public static readonly TimeOnly Opens = new(8, 0);
    public static readonly TimeOnly Closes = new(10, 0);
    public static readonly TimeSpan SlotLength = TimeSpan.FromMinutes(15);
    public const int Booths = 2;
    public const int BookingDays = 3;
}

public static class Schedule
{
    // The appointments are a plain List<Appointment> (that's what we store as
    // JSON). An extension indexer gives that list a slot lookup without
    // wrapping it in a class of our own.
    extension(IEnumerable<Appointment> appointments)
    {
        /// <summary>The patient vaccinated in this slot, or null if the slot is free.</summary>
        public Patient? this[Slot slot]
        {
            get
            {
                foreach (var a in appointments)
                {
                    if (a.Start.Date != slot.Date || a.Start.Booth != slot.Booth || slot.Time < a.Start.Time) continue;
                    int index = (int)((slot.Time - a.Start.Time) / Center.SlotLength);
                    if (index < a.Patients.Length) return a.Patients[index];
                }
                return null;
            }
        }
    }
}

public sealed class VaccinationCenter(List<Appointment> appointments, DateOnly today)
{
    // Portal users type their email in any casing, and some press "submit" twice.
    private readonly HashSet<string> knownEmails =
        [with(StringComparer.OrdinalIgnoreCase), .. appointments.Select(a => a.Email)];

    public BookingResult Book(string email, IReadOnlyList<Patient> patients)
    {
        if (knownEmails.Contains(email)) return new Duplicate();
        if (patients is []) return new NothingToBook();
        if (FindSlot(patients.Count) is not { } start) return new NoFreeSlot(patients.Count);

        var appointment = new Appointment(email, start, [.. patients]);
        appointments.Add(appointment);
        knownEmails.Add(email);
        return appointment;
    }

    // Earliest start time with `count` free slots in a row in the same booth.
    private Slot? FindSlot(int count)
    {
        for (var day = today.AddDays(1); day <= today.AddDays(Center.BookingDays); day = day.AddDays(1))
        {
            for (var time = Center.Opens; time.Add(Center.SlotLength * count) <= Center.Closes; time = time.Add(Center.SlotLength))
            {
                booth: for (int booth = 1; booth <= Center.Booths; booth++)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (appointments[new Slot(day, time.Add(Center.SlotLength * i), booth)] is not null)
                            continue booth;
                    }
                    return new Slot(day, time, booth);
                }
            }
        }
        return null;
    }
}
```

Talking points, from top to bottom:

- **`BookingResult`** is a classic result union. `Appointment` is an ordinary domain type that we
  also store in JSON, so it can't (and shouldn't) derive from a result base.
- **The extension indexer** gives the plain `List<Appointment>` a slot lookup without a wrapper
  class. We keep the list plain because that's what we serialize in step 4. Indexers are instance
  members, so the `extension` block needs a named receiver (`appointments`).
- **`with(StringComparer.OrdinalIgnoreCase)`** passes the comparer to the `HashSet` constructor,
  and `..` fills it from the existing appointments, all in one expression. Request 004 uses
  `Anna.Berger@Example.at`.
- **`continue booth;`** skips to the next booth as soon as one slot of the block is taken. Before
  C# 15 you'd need a flag, a `goto`, or a helper method.
- **`patients is []`** (and `vaccines is not []` in `Program.cs`) is a list pattern from C# 11:
  "is empty". It's not new, but some participants won't have seen it.

`Program.cs`. The appointments come from the seed file for now:

```csharp
using System.Text.Json;
using Vaccination;

// A fixed "today" (the workshop day), so the output is the same on every run.
DateOnly today = new(2026, 11, 24);

var history = Storage.Read<List<VaccinationRecord>>(Path.Combine("data", "history.json"));
var appointments = Storage.Read<List<Appointment>>(Path.Combine("data", "appointments.seed.json"));
var center = new VaccinationCenter(appointments, today);

foreach (var file in Directory.GetFiles(Path.Combine("data", "inbox"), "*.json").Order())
{
    Console.WriteLine(Path.GetFileName(file));

    PortalRequest request;
    try { request = Storage.Read<PortalRequest>(file); }
    catch (JsonException ex)
    {
        Console.WriteLine($"  rejected: {ex.Message}");
        Console.WriteLine();
        continue;
    }

    List<Patient> patients = [];
    foreach (var member in request.Members)
    {
        var person = member.ToPerson();
        List<Vaccine> vaccines = [];
        foreach (var requested in member.Vaccines)
        {
            Eligibility decision = Rules.Check(person, requested, history, today);
            Console.WriteLine($"  {person.Name,-16} {requested,-6} {decision switch
            {
                Vaccine v => $"yes: {v}",
                TooSoon t => $"not yet, earliest {t.EarliestDate:d MMM yyyy}",
                NotRecommended n => $"no: {n.Reason}",
            }}");
            if (decision is Vaccine vaccine) vaccines.Add(vaccine);
        }
        if (vaccines is not []) patients.Add(new Patient(person, [.. vaccines]));
    }

    Console.WriteLine(center.Book(request.Email, patients) switch
    {
        Appointment a => $"  => booked {a.Start.Date:ddd d MMM} {a.Start.Time:HH:mm}, booth {a.Start.Booth}",
        Duplicate => "  => already booked (duplicate request)",
        NothingToBook => "  => nothing to book",
        NoFreeSlot n => $"  => no {n.SlotsNeeded} free slots in a row, please call us",
    });
    Console.WriteLine();
}

// The day plan, one column per booth.
foreach (var day in appointments.Select(a => a.Start.Date).Distinct().Order())
{
    Console.WriteLine($"{day:ddd d MMM}  {string.Join("", Enumerable.Range(1, Center.Booths).Select(b => $"{"Booth " + b,-18}"))}");
    for (var time = Center.Opens; time < Center.Closes; time = time.Add(Center.SlotLength))
    {
        var names = Enumerable.Range(1, Center.Booths)
            .Select(booth => $"{appointments[new Slot(day, time, booth)]?.Person.Name ?? "-",-18}");
        Console.WriteLine($"{time:HH:mm}       {string.Join("", names)}");
    }
    Console.WriteLine();
}
```

**Run.** Excerpt:

```text
002-huber.json
  Thomas Huber     flu    yes: Influenza
  Thomas Huber     covid  no: COVID-19 under 60 without risk factors: ask your doctor
  Sabine Huber     flu    yes: Influenza
  Sabine Huber     covid  not yet, earliest 1 Mar 2027
  Lena Huber       flu    no: under 18, please see your paediatrician
  => booked Wed 25 Nov 08:30, booth 2
...
004-berger-again.json
  Anna Berger      flu    yes: Influenza (high dose)
  Anna Berger      covid  yes: COVID-19
  => already booked (duplicate request)
...
Wed 25 Nov  Booth 1           Booth 2
08:00       Karl Wimmer       Anna Berger
08:15       Maria Steiner     Petra Lang
08:30       Eva Bauer         Thomas Huber
08:45       -                 Sabine Huber
09:00       -                 -
09:15       -                 -
09:30       -                 -
09:45       -                 -
```

The Huber family needs two slots in a row. At 08:15, booth 1 is free but 08:30 isn't (Eva Bauer),
and booth 2 is taken (Petra Lang). So the search moves on to 08:30 in booth 2.

**Break it on purpose:**

1. Replace `continue booth;` with a plain `continue;`. It now only continues the inner loop, so
   the first booth is always "free" and every request lands at 08:00 in booth 1. The day plan only
   shows one name per slot, so it looks broken in several places: Anna Berger, Thomas Huber and
   Maria Steiner disappear, and Sabine Huber shows up at 08:15. Everyone was double-booked over Karl
   Wimmer. Put `booth` back.
2. Remove `with(StringComparer.OrdinalIgnoreCase),`. Request 004 books Anna Berger a second time
   (08:15, booth 1), and Maria Steiner moves to 08:45. Put it back.

---

## Step 4: Store the appointments

**Discuss first:** recap section (3) of `14_JsonUnions` (closed hierarchy, inferred `$type`), and
compare it with `CSharp14Way.cs`.

**Goal:** appointments survive a restart, and running the app again doesn't book anyone twice.

### Model it first

> **Requirements**
>
> - Appointments must survive a restart. A JSON file is enough.
> - A human should be able to read the file.
> - Adding a vaccine later must not require changes to the storage code.

**Ask:**

1. What does an appointment with two vaccines look like in JSON? Sketch it.
2. How does the reader know that `{ "highDose": true }` is an `Influenza`?
3. What did this cost in C# 14, and what happens if someone forgets a step?

**Answers you'll likely hear, and how to respond:**

- *"A `kind` property that we write and read ourselves."* Hand-written mapping code in both
  directions, and one more place to update for every new vaccine.
- *"`[JsonDerivedType]` attributes on the base class."* The C# 14 way. It works, but it's a second
  list of subtypes next to the class declarations. Forget one and it fails at runtime with
  `NotSupportedException`.
- *"Make `Vaccine` a union instead."* Ask what a union writes: the bare case value, without a
  discriminator. That worked for the portal input in step 1, where the two shapes are clearly
  different. Here it doesn't: `Covid` has no properties at all, so there is nothing to recognize it
  by.

**Where we land:** nothing new to model. The closed hierarchy from step 2 already has
`[JsonPolymorphic(InferClosedTypePolymorphism = true)]`, because we needed it to read
`history.json`. It writes `"$type"`, and the list of subtypes comes from the compiler. This step
only adds saving.

### Code

Change the top of `Program.cs`. The first run starts from the seed file:

```csharp
string appointmentsFile = Path.Combine("data", "appointments.json");
if (!File.Exists(appointmentsFile)) File.Copy(Path.Combine("data", "appointments.seed.json"), appointmentsFile);

var history = Storage.Read<List<VaccinationRecord>>(Path.Combine("data", "history.json"));
var appointments = Storage.Read<List<Appointment>>(appointmentsFile);
```

After the closing brace of the `foreach` over the inbox files, before the day plan:

```csharp
Storage.Write(appointmentsFile, appointments);
```

**Run twice.** The first run is the same as in step 3. On the second run every request is a
duplicate:

```text
001-berger.json
  Anna Berger      flu    yes: Influenza (high dose)
  Anna Berger      covid  yes: COVID-19
  => already booked (duplicate request)
```

Open `data/appointments.json` and show the `$type` entries. Anna Berger's appointment contains:

```json
"vaccines": [
  {
    "$type": "Influenza",
    "highDose": true
  },
  {
    "$type": "Covid"
  }
]
```

**Break it on purpose:** in step 2 we saw the read side fail without
`[JsonPolymorphic(InferClosedTypePolymorphism = true)]`. Now show the write side, which is worse.
Remove the attribute and add this line right after `DateOnly today = ...;` (the app crashes when it
reads `history.json` a few lines later, so the line must come first):

```csharp
Console.WriteLine(JsonSerializer.Serialize<Vaccine[]>([new Influenza(true), new Covid()], Storage.Options));
```

It prints two empty objects, without any warning:

```text
[
  {},
  {}
]
```

Then the expected crash follows. That is silent data loss: had the app saved
`appointments.json` like this, the next start would fail on reading it. Remove the line and put the
attribute back.

Aside: `[JsonPolymorphic(..., TypeDiscriminatorPropertyName = "type")]` renames `$type` and still
works with inference. The discriminator values remain the C# type names. If you show this, update
`history.json` and `appointments.seed.json` too.

To start over at any time: `rm data/appointments.json`.

---

## Step 5: A new vaccine is approved (RSV)

**Discuss first:** nothing new. This step is the payoff of the day.

**Goal:** offer RSV vaccination. Request 006 (Maria Steiner, 80) has been asking for it all day.

### Model it first

Before you touch the code, show the new requirements and ask the group to **list every place in
the code that has to change**. Write the list on the whiteboard.

> **Requirements**
>
> - RSV vaccination is approved.
> - From 75, or from 60 with risk factors.
> - A single dose: anyone who already had it can't book it again.
> - The portal sends the code `"rsv"`.

Typical lists contain: a new record, the eligibility rules, the display text, the portal code
mapping, the JSON registration, and "search the code for every switch over vaccines".

Then do the steps below and compare with the whiteboard:

- **The compiler** finds two places: the display text and the rules.
- **JSON** needs no change at all.
- **The portal code mapping** is found by nobody but the humans, because the compiler can't check
  strings. Discuss whether it's worth making that checkable too. For example, an exhaustive switch
  from `Vaccine` to its portal code would turn a missing code into a compile error. For three
  vaccines it may not be worth it; for thirty it would be.

### Code

First `rm data/appointments.json`. Step 4 already booked Maria (flu only), so without the reset her
request would just be a duplicate.

1. Add the case to `Domain.cs`:

   ```csharp
   public sealed record Rsv : Vaccine;
   ```

2. Build. The compiler produces the to-do list:

   ```text
   Domain.cs(22,54):      error CS8509: ... the pattern 'Vaccination.Rsv' is not covered.
   Eligibility.cs(31,23): error CS8509: ... the pattern 'Vaccination.Rsv' is not covered.
   ```

3. Fix `Vaccine.ToString()`:

   ```csharp
   Rsv => "RSV",
   ```

4. Fix `Rules.Check` with the RSV rule (a single dose, from 75, or from 60 with risk factors).
   The arms go into the `wanted switch`, after `Covid c => c,`:

   ```csharp
   Rsv when LastDose<Rsv>(past) is not null
       => new NotRecommended("RSV is a single dose, already vaccinated"),
   Rsv when age < 60 || (age < 75 && !person.RiskFactors)
       => new NotRecommended("RSV: from 75, or from 60 with risk factors"),
   Rsv r
       => r,
   ```

5. Run. Maria still gets "'rsv' is not offered here". The portal code mapping at the top of
   `Rules.Check` is a `string` switch with a fallback, and the compiler can't check strings. Add
   the line after `"covid" => new Covid(),`:

   ```csharp
   "rsv" => new Rsv(),
   ```

6. `rm data/appointments.json` once more (the run in 5. booked Maria without RSV), then run again.

**Run.** Excerpt:

```text
006-steiner.json
  Maria Steiner    rsv    yes: RSV
  Maria Steiner    flu    yes: Influenza (high dose)
  => booked Wed 25 Nov 08:15, booth 1
```

**What we didn't touch:** JSON. Thanks to `closed` and `InferClosedTypePolymorphism`, RSV is
stored and loaded with `"$type": "Rsv"` without further changes. Open `data/appointments.json` and
search for `Rsv` to show it. In C# 14 you would maintain a
`[JsonDerivedType]` list on the base class. Forget to add `Rsv` there and it compiles fine, then
throws `NotSupportedException` at runtime the first time an RSV appointment is saved
(`14_JsonUnions`, `CSharp14Way.cs`).

---

## Final modeling exercise: the vaccination day (no code)

This one is for discussion only. The sample doesn't implement it, so the group designs it from
scratch.

> **Requirements**
>
> - On the day, an appointment goes through states: booked, checked in (with the time), vaccinated
>   (with the time and a lot number for each vaccine), deferred (with a reason, for example fever;
>   the person has to book again), or no-show.
> - The status is stored in `appointments.json`.
> - Reports: no-shows per day, and every lot number used.

**Ask:**

1. Union or closed hierarchy?
2. Does the status belong to the appointment, or to each patient? A family member with a fever
   can't be vaccinated, but the others can.
3. How do we store "a lot number for each vaccine"?
4. How does a status change look in code?

**Answers you'll likely hear, and how to respond:**

- *"An enum `Status` plus nullable `CheckedInAt`, `LotNumbers` and `DeferralReason` fields."* The
  classic mix of nullable fields that `04_DomainModeling` replaces: the types don't say which
  fields belong to which status.
- *"A union of status records."* Fine in memory, but the status is stored. A union writes the bare
  value without a discriminator, and `Booked` and `NoShow` have no properties at all. System.Text.Json
  refuses to set up such a union with the structural classifier: "the case type ... can never be
  selected uniquely".

**A possible answer.** It's the same pattern as `Vaccine`: we own the types and store them, so it's
a closed hierarchy with inferred polymorphism, plus an `AppointmentStatus Status` property on the
record that holds it. This compiles and round-trips through JSON:

```csharp
[JsonPolymorphic(InferClosedTypePolymorphism = true)]
public closed record AppointmentStatus;
public sealed record Booked : AppointmentStatus;
public sealed record CheckedIn(TimeOnly At) : AppointmentStatus;
public sealed record Vaccinated(TimeOnly At, string[] LotNumbers) : AppointmentStatus;
public sealed record Deferred(string Reason) : AppointmentStatus;
public sealed record NoShow : AppointmentStatus;

// A status change is a copy with a new status:
appointment = appointment with { Status = new CheckedIn(new TimeOnly(8, 5)) };
```

It's a starting point, not the answer. Questions 2 and 3 should poke holes in it: `Status` probably
belongs on `Patient`, and a `string[]` of lot numbers only matches the vaccines by position. For a
code version, write the transitions as a switch over a tuple `(AppointmentStatus, Event)`.

---

## Exercises for the participants

1. **Make `NoFreeSlot` happen.** In `Booking.cs`, set `Closes = new(8, 30)` and `BookingDays = 1`,
   then `rm data/appointments.json` and run. The Huber family gets "no 2 free slots in a row".
2. **Add a fourth vaccine**, for example pneumococcal (from 60, a single dose). Count how many
   places the compiler sends you to and how many it can't (the portal mapping).
3. **Show the eligibility of all family members side by side**, and use a switch over a tuple
   `(Eligibility, Eligibility)` for "flu and COVID-19 in the same appointment".
4. **Make it strict end to end:** what happens when an inbox file contains both `person` and
   `members`? Try it, then decide whether the app should reject it.
