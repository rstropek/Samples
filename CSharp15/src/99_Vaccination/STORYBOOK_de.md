# Storybook: das Impfzentrum

Ein Drehbuch für Vortragende, um `src/99_Vaccination` im ganztägigen C#-15-Workshop live zu
bauen. Das Beispiel entsteht in fünf Schritten, und jeder Schritt folgt demselben Rhythmus:

1. **Vorher besprechen:** die kleinen Beispiele, die die Features einführen.
2. **Erst modellieren:** Zeig die Anforderungen und lass die Gruppe die Typen entwerfen, bevor
   Code geschrieben wird. Zu jedem Schritt stehen die Antworten, die du wahrscheinlich hören wirst
   (oft der C#-14-Weg), wie du darauf reagierst und worauf wir uns am Ende einigen.
3. **Programmieren und ausführen:** Bau den Schritt und vergleich das Ergebnis mit dem Entwurf der
   Gruppe.
4. **Absichtlich kaputt machen:** kleine Experimente, die zeigen, wovor jedes Feature schützt.

Der Tag endet mit einer Modellierungsübung ohne Code (dem Impftag selbst).

## Die Geschichte

Es ist Grippe- und COVID-19-Saison. Eine Stadt betreibt ein Impfzentrum, und ihr Online-Portal legt
pro Terminanfrage eine JSON-Datei in einem Ordner ab. Unsere Konsolen-App:

1. liest die Anfragen (eine Person oder eine ganze Familie, in einem Format, das wir nicht
   bestimmen),
2. prüft jede gewünschte Impfung gegen das Alter und den Impfverlauf der Person,
3. bucht aufeinanderfolgende Slots (eine Familie wird nacheinander in derselben Kabine geimpft),
4. speichert alle Termine in einer JSON-Datei und gibt den Tagesplan aus.

Mitten in der Saison kommt eine RSV-Impfung dazu. Der Compiler sagt uns, was wir anpassen müssen.

Die Impfregeln sind vereinfachte Demo-Regeln. Sag das früh, damit niemand anfängt, über echte
Impfempfehlungen zu diskutieren.

## Features und wo sie vorkommen

| C# 15 / .NET 11 Feature | Wo im Beispiel | Schritt |
|---|---|---|
| Union aus bestehenden Typen | `PortalRequest(SingleRequest, FamilyRequest)` | 1 |
| Member im Union-Body | `PortalRequest.Email` und `.Members` mit Switch über `this` | 1 |
| JSON: Union mit Structural Classifier | Das Portal-Format hat kein Typ-Feld, daher wird der Fall anhand der Property-Namen gewählt | 1 |
| JSON: strikte Deserialisierung | `RespectRequiredConstructorParameters`, `RespectNullableAnnotations` | 1 |
| Geschlossene Hierarchie | `closed record Vaccine` mit `Influenza`, `Covid` (später `Rsv`) | 2 |
| Vollständiger Switch, `when`-Guards, kein `_`-Arm | `Rules.Check`, `Vaccine.ToString` | 2 |
| Union mit einer geschlossenen Hierarchie als Fall | `Eligibility(Vaccine, TooSoon, NotRecommended)` | 2 |
| JSON: abgeleitete Polymorphie für geschlossene Typen | `history.json`, `appointments.json` (`$type`) | 2, 4 |
| Union als Ergebnistyp | `BookingResult(Appointment, Duplicate, NothingToBook, NoFreeSlot)` | 3 |
| Argumente in Collection Expressions | `HashSet<string>` mit `with(StringComparer.OrdinalIgnoreCase)` | 3 |
| Extension-Indexer | `appointments[slot]` auf einer einfachen `List<Appointment>` | 3 |
| `continue` mit Label | `continue booth;` in der Slot-Suche | 3 |
| Sicherheit beim Hinzufügen eines Falls | RSV: zwei Compilerfehler, keine JSON-Änderungen | 5 |

**Bewusst weggelassen**, weil die Domäne keinen natürlichen Platz dafür hat:

- **Eigene Unions ohne Boxing** (`13_CustomUnions`): Es gibt hier keinen Hot Path.
- **Runtime Async** (`08`/`09`): Die App macht keine nennenswerte asynchrone Arbeit.
- **Memory Safety** (`16_MemorySafety`): Dafür gibt es ein eigenes Beispiel.

Das laut zu sagen ist selbst schon eine Lektion: Nicht jedes Feature gehört in jede Codebasis.

## Vorbereitung

Leg neben dem fertigen Beispiel ein leeres Projekt an und kopier die Demo-Daten:

```bash
cd src
mkdir 99_Live
cp 99_Vaccination/99_Vaccination.csproj 99_Live/
cp -R 99_Vaccination/data 99_Live/
rm -f 99_Live/data/appointments.json
cd 99_Live
```

Das Projekt bekommt `net11.0`, Nullable und Implicit Usings aus der `Directory.Build.props` des
Repos. Zeig die `.csproj`: Sie macht aus den beiden Warnungen "Switch ist nicht vollständig"
Fehler. CS8509 bedeutet, dass ein Fall gar keinen Arm hat. CS8846 bedeutet, dass ein Fall nur Arme
mit einem `when`-Guard hat. Erwähn es jetzt; es zahlt sich in Schritt 2 und 5 aus.

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

Geh kurz durch `data/`:

- `inbox/*.json`: sechs Portal-Anfragen. 001 ist eine Einzelperson, 002 eine Familie, 003 jemand,
  der kürzlich geimpft wurde, 004 ein Duplikat von 001 mit anderer Groß-/Kleinschreibung der
  E-Mail, bei 005 fehlt das Geburtsdatum, 006 möchte RSV.
- `history.json`: frühere Impfungen, importiert aus dem e-Impfpass.
- `appointments.seed.json`: drei Termine, die es schon vor der Demo gab.

Starte die App immer aus dem Projektordner (`dotnet run`), weil sie `data/` relativ zum aktuellen
Verzeichnis liest. Das fertige Beispiel in `src/99_Vaccination` ist der Stand nach Schritt 5 (bis
auf den Kopfkommentar in `Program.cs`). Zwischenstände der einzelnen Schritte gibt es nicht; die
Codeblöcke unten sind vollständig.

---

## Schritt 1: Die Portal-Anfragen lesen

**Vorher besprechen:** `01_Basics`, `11_UnionWithMembers` und Abschnitt (2) von `14_JsonUnions`.

**Ziel:** jede Datei aus der Inbox in eine typisierte Anfrage lesen, egal ob für eine Person oder
eine Familie.

### Erst modellieren

Zeig die Anforderungen und lass die Gruppe die C#-Typen entwerfen. Sammle die Antworten, bevor du
sie kommentierst.

> **Anforderungen**
>
> - Das Portal der Stadt schickt eine JSON-Datei pro Buchung. Wir können das Format nicht ändern.
> - Eine Buchung ist entweder für eine Person oder für eine Familie:
>
>   ```json
>   { "email": "...", "person":  { "name": "...", "birthDate": "...", ... } }
>   { "email": "...", "members": [ { ... }, { ... } ] }
>   ```
>
> - Kein Feld sagt, welche der beiden Varianten es ist.
> - Der Rest der App braucht nur eine E-Mail-Adresse und eine Liste von Personen.
> - Fehlerhafte Dateien müssen abgelehnt werden, nicht halb eingelesen.

**Frag:**

1. Welche C#-Typen würdet ihr für eine Anfrage schreiben?
2. Woher weiß der Deserializer, welche der beiden Varianten er gerade liest?
3. Wo werden aus den beiden Varianten "eine E-Mail und eine Liste von Personen"?
4. Was passiert mit einer Datei, in der ein Feld fehlt?

**Antworten, die du wahrscheinlich hörst, und wie du reagierst:**

- *"Eine Klasse mit einer nullable `Person` und nullable `Members`."* Frag, was es bedeutet, wenn
  beide null sind oder beide gesetzt. Der Typ erlaubt Zustände, die das Portal nie schickt, und
  jeder, der ihn verwendet, braucht Null-Prüfungen.
- *"Eine abstrakte Basisklasse, zwei Unterklassen und ein eigener `JsonConverter`."* Das ist die
  C#-14-Antwort, und sie funktioniert. Frag, wer den Converter schreibt und wartet, der sich die
  Property-Namen ansieht (`CSharp14Way.cs` in `14_JsonUnions` enthält einen).
- *"Eine geschlossene Hierarchie mit `$type`."* Ein guter Instinkt für unsere eigenen Daten, aber
  das JSON des Portals hat kein `$type`, und wir können keins hinzufügen.
- *"Eine Einzelperson als Familie mit einem Mitglied behandeln."* Genau das macht die App intern
  (`Members`). Aber das JSON kommt trotzdem in zwei Varianten, also muss das Einlesen beide
  kennen.

**Worauf wir uns einigen:** zwei einfache Records, einer pro Variante, und `union
PortalRequest(SingleRequest, FamilyRequest)`. Die beiden haben keine gemeinsame Basisklasse, also
ist das eine Union aus bestehenden Typen. Der Structural Classifier wählt den Fall anhand der
Property-Namen. Member im Union-Body machen aus beiden Varianten `Email` und `Members`, sodass
sonst nirgends in der App über die Variante geswitcht wird. Strikte JSON-Optionen lehnen Dateien
mit fehlenden Feldern ab.

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

Optionaler Umweg beim Live-Coding: Schreib die Union zuerst ohne Body und setz den Switch in
`Program.cs`, zum Beispiel `string email = request switch { SingleRequest s => s.Email,
FamilyRequest f => f.Email };`. Verschieb ihn dann wie oben gezeigt in den Union-Body, um zu
zeigen, dass eine Union Member haben kann. Im Body switchst du über `this`, nicht über `Value`:
`Value` ist `object?`, ein Switch darüber ist also nicht vollständig. Die `Program.cs` unten
verwendet schon die Member.

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

**Ausführen** (`dotnet run`). Ausschnitt:

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

**Absichtlich kaputt machen:**

1. Entfern das Attribut `[JsonUnion(...)]`. Zur Laufzeit wird jede Datei abgelehnt:
   `JSON value type 'Object' is ambiguous for union type 'Vaccination.PortalRequest' because
   multiple case types can use this value type.` Zwei Objekt-Fälle brauchen den Structural
   Classifier.
2. Setz das Attribut wieder ein. Entfern die beiden `Respect...`-Zeilen in `Storage.cs`. Anfrage
   005 wird jetzt akzeptiert, und Peter Novak ist "born 1 Jan 0001". Vorgeschmack auf Schritt 2:
   Sobald es die Impfregeln gibt, wäre er 2025 Jahre alt und bekäme eine Hochdosis-Grippeimpfung.
   Strikte Optionen sind die billige Variante der Eingabeprüfung an einer Schnittstelle, die du
   nicht kontrollierst. Setz die Zeilen wieder ein.

---

## Schritt 2: Impfstoffe und Impfberechtigung

**Vorher besprechen:** `12_ClosedHierarchies`, `04_DomainModeling`, `05_PatternShowcase` und
Abschnitt (3) von `14_JsonUnions`.

**Ziel:** für jede gewünschte Impfung entscheiden, ob die Person sie bekommt, und wenn nicht,
warum nicht.

### Erst modellieren

> **Anforderungen** (vereinfachte Demo-Regeln)
>
> - Das Zentrum bietet Grippe- und COVID-19-Impfungen an. Weitere Impfstoffe können dazukommen.
> - Ab 60 gibt es den Hochdosis-Grippeimpfstoff.
> - Grippe: einmal pro Saison. Die Saison beginnt am 1. September.
> - COVID-19: nur ab 60 oder mit Risikofaktoren, und höchstens einmal in 12 Monaten.
> - Niemand unter 18. Kinder gehen zum Kinderarzt.
> - Wenn die Antwort Nein ist, muss die Person erfahren, warum. Heißt es "noch nicht", braucht sie
>   das früheste Datum.

**Frag:**

1. Wie stellen wir die Impfstoffe dar?
2. Was gibt `Check(person, vaccine)` zurück?

**Antworten, die du wahrscheinlich hörst, zu den Impfstoffen:**

- *"Ein Enum `VaccineKind { Flu, Covid }`."* Zwei Probleme:
  - Wohin kommt `HighDose`? Ein `bool HighDose` neben dem Enum ist für COVID-19 sinnlos.
  - Ein Switch über ein Enum ist nie vollständig. Selbst wenn jeder benannte Wert behandelt ist,
    warnt der Compiler mit CS8524, weil ein Enum jeden `int` enthalten kann. Teams fügen dann
    `_ => throw ...` hinzu, und das versteckt den nächsten neuen Impfstoff vor dem Compiler.
- *"Eine Basisklasse mit Unterklassen."* Nah dran. Aber ohne `closed` kann der Compiler nicht
  wissen, dass die Liste vollständig ist, also braucht jeder Switch weiterhin einen `_`-Arm.
- *"Eine Union aus Records."* Möglich, aber uns gehören alle Typen, sie teilen Verhalten
  (`ToString`), und in Schritt 4 speichern wir sie mit einem Typ-Discriminator. Genau dafür sind
  geschlossene Hierarchien da.

**Antworten, die du wahrscheinlich hörst, zum Ergebnis:**

- *"`bool`."* Grund und Datum gehen verloren.
- *"`(bool Ok, string? Reason, DateOnly? EarliestDate)`", oder eine Klasse mit nullable Feldern.*
  Frag, welche Kombinationen gültig sind. `Ok = true` mit einem Grund? Der Typ sagt es nicht.
- *"Eine Exception werfen."* "Nicht berechtigt" ist eine normale Antwort, kein Fehler. Und die
  Signatur sagt dem Aufrufer nicht, mit welchen Exceptions er rechnen muss.
- *"`Result<Vaccine, string>`."* Besser. Aber "noch nicht" trägt ein Datum und "nicht empfohlen"
  einen Text. Wenn man beides in einen `string` quetscht, geht das Datum verloren.

**Worauf wir uns einigen:**

- **`closed record Vaccine`** mit `Influenza(bool HighDose)` und `Covid`. Jeder Fall trägt nur
  seine eigenen Daten, und Switches sind ohne `_` vollständig.
- **`union Eligibility(Vaccine, TooSoon, NotRecommended)`.** Ein "Ja" wird mit dem Impfstoff
  beantwortet, der verabreicht wird: Die Regeln machen aus einer Grippe-Anfrage für über 60-Jährige
  eine Hochdosis-Impfung. `Vaccine` ist ein bestehender Typ und selbst eine geschlossene
  Hierarchie, kann also nicht zusätzlich von einer Ergebnis-Basisklasse ableiten. Genau hier passt
  eine Union besser als eine zweite geschlossene Hierarchie.

### Code

`Domain.cs` (erste Version):

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

Weise darauf hin:

- Auf jeden `when`-Arm folgt ein Arm ohne Guard für denselben Typ (`Influenza => ...`). Arme mit
  Guard zählen nicht für die Vollständigkeit. Ohne den Arm ohne Guard bekommst du CS8846 ("a
  pattern with a 'when' clause might successfully match this value"), in diesem Projekt ein
  Fehler. Das Beispiel in der Meldung kann einen anderen Typ nennen als den, den du entfernt hast:
  Der Compiler zeigt die erste Lücke, die er findet.
- `Influenza => new Influenza(HighDose: age >= 60)` wird implizit in `Eligibility` umgewandelt. Es
  braucht weder Factory-Methode noch Wrapper.
- `Vaccine.ToString()` ist ein `sealed override` im geschlossenen Basis-Record. Ohne `sealed` würde
  jeder abgeleitete Record sein eigenes `ToString()` generieren und unseres ignorieren. Weil es
  über `this` switcht, nimmt es an der Vollständigkeitsprüfung teil (siehe Schritt 5).

Ergänz `ToPerson` in `PortalPerson` in `PortalRequest.cs`:

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

**Ausführen.** Ausschnitt. Die Familie Huber zeigt alle drei Fälle der Union:

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

**Absichtlich kaputt machen:**

1. Lösch alle drei `Covid`-Arme in `Rules.Check`. Der Build schlägt mit CS8509 fehl ("the pattern
   'Vaccination.Covid' is not covered"). Setz sie wieder ein.
2. Entfern `[JsonPolymorphic(InferClosedTypePolymorphism = true)]`. Das Lesen von `history.json`
   schlägt jetzt fehl mit `NotSupportedException: Deserialization of interface or abstract types
   is not supported. Type 'Vaccination.Vaccine'.` Ein geschlossener Typ ist implizit abstrakt. Ohne
   die Ableitung kennt System.Text.Json seine Untertypen nicht. Setz das Attribut wieder ein.

---

## Schritt 3: Die Slots buchen

**Vorher besprechen:** `07_CollectionExprArgs` und `15_IndexersAndLabels`. `BookingResult` braucht
nichts Neues: Es ist dieselbe Art von Union wie `Eligibility` in Schritt 2.

**Ziel:** alle, die mindestens ein "Ja" bekommen haben, in aufeinanderfolgende Slots buchen und den
Tagesplan ausgeben.

### Erst modellieren

> **Anforderungen**
>
> - Ein Slot dauert 15 Minuten, von 08:00 bis 10:00, in zwei Kabinen. Gebucht wird für die
>   nächsten drei Tage.
> - Eine Familie wird nacheinander in derselben Kabine geimpft, braucht also aufeinanderfolgende
>   freie Slots.
> - Gebucht wird der frühestmögliche Zeitpunkt.
> - Manche drücken zweimal auf "Absenden", manchmal mit anderer Groß-/Kleinschreibung der E-Mail.
>   Jede E-Mail wird nur einmal gebucht.
> - Eine Buchung kann aus mehreren Gründen scheitern, und jeder Grund bekommt eine eigene Meldung.
> - Die Termine sind eine `List<Appointment>`, weil wir genau das in Schritt 4 als JSON speichern.

**Frag:**

1. Was gibt `Book(...)` zurück?
2. Wie prüfen wir, ob ein Slot belegt ist?
3. Wie suchen wir einen Block freier Slots?
4. Wie erkennen wir doppelte E-Mails?

**Antworten, die du wahrscheinlich hörst, und wie du reagierst:**

- **Rückgabetyp:**
  - *`Appointment?`*: null heißt, es hat nicht geklappt, aber warum?
  - *`bool TryBook(..., out Appointment? appointment, out string? reason)`*: Gründe als Strings, und
    jeder Aufrufer muss daran denken, sie zu prüfen.
  - *Exceptions*: dasselbe Problem wie in Schritt 2.

  Dann frag: Was passiert mit allen Aufrufern, wenn ein neuer Grund fürs Scheitern dazukommt? Mit
  einer Union und einem vollständigen Switch listet der Compiler sie auf.
- **Slot-Abfrage:** eine `Schedule`-Klasse, die die Liste kapselt (mehr Code, und JSON muss dann die
  Hülle speichern), oder dieselbe LINQ-Abfrage an mehreren Stellen. Ein Extension-Indexer gibt der
  einfachen Liste eine Abfrage `appointments[slot]`, ohne Hülle.
- **Suche:** verschachtelte Schleifen mit einem `found`-Flag, oder eine Hilfsmethode, die `bool`
  zurückgibt. Beides funktioniert. Ein `continue booth;` mit Label sagt genau, was wir meinen:
  Diese Kabine passt nicht, probier die nächste.
- **Duplikate:** `ToLowerInvariant()` vor jeder Abfrage, was man leicht an einer Stelle vergisst,
  oder ein Set, das von Anfang an ohne Rücksicht auf Groß-/Kleinschreibung vergleicht.

**Worauf wir uns einigen:** `union BookingResult(Appointment, Duplicate, NothingToBook, NoFreeSlot)`,
ein Extension-Indexer `this[Slot]` auf `IEnumerable<Appointment>`, `continue booth;` und ein
`HashSet<string>`, erzeugt mit `with(StringComparer.OrdinalIgnoreCase)`.

### Code

Ergänz in `Domain.cs`:

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

Gesprächspunkte, von oben nach unten:

- **`BookingResult`** ist eine klassische Ergebnis-Union. `Appointment` ist ein ganz normaler
  Domänentyp, den wir auch als JSON speichern. Er kann (und soll) daher nicht von einer
  Ergebnis-Basisklasse ableiten.
- **Der Extension-Indexer** gibt der einfachen `List<Appointment>` eine Slot-Abfrage ohne eigene
  Hüllklasse. Wir lassen die Liste einfach, weil wir genau sie in Schritt 4 serialisieren. Indexer
  sind Instanz-Member, daher braucht der `extension`-Block einen benannten Receiver
  (`appointments`).
- **`with(StringComparer.OrdinalIgnoreCase)`** übergibt den Comparer an den Konstruktor von
  `HashSet`, und `..` befüllt es mit den bestehenden Terminen, alles in einem Ausdruck. Anfrage 004
  verwendet `Anna.Berger@Example.at`.
- **`continue booth;`** springt zur nächsten Kabine, sobald ein Slot des Blocks belegt ist. Vor
  C# 15 hättest du ein Flag, ein `goto` oder eine Hilfsmethode gebraucht.
- **`patients is []`** (und `vaccines is not []` in `Program.cs`) ist ein List Pattern aus C# 11:
  "ist leer". Das ist nicht neu, aber manche im Publikum haben es vielleicht noch nicht gesehen.

`Program.cs`. Die Termine kommen vorerst aus der Seed-Datei:

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

**Ausführen.** Ausschnitt:

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

Die Familie Huber braucht zwei Slots hintereinander. Um 08:15 ist Kabine 1 frei, aber 08:30 nicht
(Eva Bauer), und Kabine 2 ist belegt (Petra Lang). Also geht die Suche weiter zu 08:30 in Kabine 2.

**Absichtlich kaputt machen:**

1. Ersetz `continue booth;` durch ein einfaches `continue;`. Es setzt jetzt nur die innere Schleife
   fort, die erste Kabine ist also immer "frei", und jede Anfrage landet um 08:00 in Kabine 1. Der
   Tagesplan zeigt nur einen Namen pro Slot und sieht deshalb an mehreren Stellen kaputt aus: Anna
   Berger, Thomas Huber und Maria Steiner verschwinden, und Sabine Huber taucht um 08:15 auf. Alle
   wurden doppelt über Karl Wimmer gebucht. Setz `booth` wieder ein.
2. Entfern `with(StringComparer.OrdinalIgnoreCase),`. Anfrage 004 bucht Anna Berger ein zweites Mal
   (08:15, Kabine 1), und Maria Steiner rutscht auf 08:45. Setz es wieder ein.

---

## Schritt 4: Die Termine speichern

**Vorher besprechen:** Wiederholung von Abschnitt (3) aus `14_JsonUnions` (geschlossene Hierarchie,
abgeleitetes `$type`) und der Vergleich mit `CSharp14Way.cs`.

**Ziel:** Die Termine überleben einen Neustart, und ein erneuter Start der App bucht niemanden
doppelt.

### Erst modellieren

> **Anforderungen**
>
> - Termine müssen einen Neustart überleben. Eine JSON-Datei reicht.
> - Ein Mensch soll die Datei lesen können.
> - Ein später hinzugefügter Impfstoff darf keine Änderung am Speichercode erfordern.

**Frag:**

1. Wie sieht ein Termin mit zwei Impfstoffen als JSON aus? Skizziert es.
2. Woher weiß der Leser, dass `{ "highDose": true }` eine `Influenza` ist?
3. Was hat das in C# 14 gekostet, und was passiert, wenn jemand einen Schritt vergisst?

**Antworten, die du wahrscheinlich hörst, und wie du reagierst:**

- *"Eine `kind`-Property, die wir selbst schreiben und lesen."* Handgeschriebener Mapping-Code in
  beide Richtungen, und eine Stelle mehr, die bei jedem neuen Impfstoff angepasst werden muss.
- *"`[JsonDerivedType]`-Attribute an der Basisklasse."* Der C#-14-Weg. Das funktioniert, ist aber
  eine zweite Liste der Untertypen neben den Klassendeklarationen. Vergisst man einen, scheitert es
  zur Laufzeit mit `NotSupportedException`.
- *"Machen wir `Vaccine` doch zu einer Union."* Frag, was eine Union schreibt: den reinen Wert des
  Falls, ohne Discriminator. Für die Portal-Eingabe in Schritt 1 hat das funktioniert, weil sich
  die beiden Varianten klar unterscheiden. Hier nicht: `Covid` hat überhaupt keine Properties, es
  gibt also nichts, woran man es erkennen könnte.

**Worauf wir uns einigen:** Es gibt nichts Neues zu modellieren. Die geschlossene Hierarchie aus
Schritt 2 hat schon `[JsonPolymorphic(InferClosedTypePolymorphism = true)]`, weil wir das zum Lesen
von `history.json` gebraucht haben. Sie schreibt `"$type"`, und die Liste der Untertypen kommt vom
Compiler. Dieser Schritt ergänzt nur das Speichern.

### Code

Ändere den Anfang von `Program.cs`. Der erste Start beginnt mit der Seed-Datei:

```csharp
string appointmentsFile = Path.Combine("data", "appointments.json");
if (!File.Exists(appointmentsFile)) File.Copy(Path.Combine("data", "appointments.seed.json"), appointmentsFile);

var history = Storage.Read<List<VaccinationRecord>>(Path.Combine("data", "history.json"));
var appointments = Storage.Read<List<Appointment>>(appointmentsFile);
```

Nach der schließenden Klammer der `foreach`-Schleife über die Inbox-Dateien, vor dem Tagesplan:

```csharp
Storage.Write(appointmentsFile, appointments);
```

**Zweimal ausführen.** Der erste Lauf entspricht Schritt 3. Beim zweiten Lauf ist jede Anfrage ein
Duplikat:

```text
001-berger.json
  Anna Berger      flu    yes: Influenza (high dose)
  Anna Berger      covid  yes: COVID-19
  => already booked (duplicate request)
```

Öffne `data/appointments.json` und zeig die `$type`-Einträge. Der Termin von Anna Berger enthält:

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

**Absichtlich kaputt machen:** In Schritt 2 haben wir gesehen, wie das Lesen ohne
`[JsonPolymorphic(InferClosedTypePolymorphism = true)]` scheitert. Jetzt zeig die Schreibseite, die
noch schlimmer ist. Entfern das Attribut und füg diese Zeile direkt nach `DateOnly today = ...;`
ein (die App stürzt ein paar Zeilen später beim Lesen von `history.json` ab, deshalb muss die Zeile
davor stehen):

```csharp
Console.WriteLine(JsonSerializer.Serialize<Vaccine[]>([new Influenza(true), new Covid()], Storage.Options));
```

Sie gibt zwei leere Objekte aus, ohne jede Warnung:

```text
[
  {},
  {}
]
```

Danach folgt der erwartete Absturz. Das ist stiller Datenverlust: Hätte die App
`appointments.json` so gespeichert, würde der nächste Start beim Einlesen scheitern. Entfern die
Zeile und setz das Attribut wieder ein.

Nebenbei: `[JsonPolymorphic(..., TypeDiscriminatorPropertyName = "type")]` benennt `$type` um und
funktioniert trotzdem mit der Ableitung. Die Werte des Discriminators bleiben die C#-Typnamen. Wenn
du das zeigst, pass auch `history.json` und `appointments.seed.json` an.

Neu anfangen kannst du jederzeit mit `rm data/appointments.json`.

---

## Schritt 5: Ein neuer Impfstoff wird zugelassen (RSV)

**Vorher besprechen:** nichts Neues. Dieser Schritt ist der Höhepunkt des Tages.

**Ziel:** RSV-Impfungen anbieten. Anfrage 006 (Maria Steiner, 80) fragt schon den ganzen Tag
danach.

### Erst modellieren

Bevor du den Code anfasst, zeig die neuen Anforderungen und bitte die Gruppe, **jede Stelle im Code
aufzulisten, die sich ändern muss**. Schreib die Liste aufs Whiteboard.

> **Anforderungen**
>
> - Die RSV-Impfung ist zugelassen.
> - Ab 75, oder ab 60 mit Risikofaktoren.
> - Eine einzige Dosis: Wer sie schon hatte, kann sie nicht noch einmal buchen.
> - Das Portal schickt den Code `"rsv"`.

Typische Listen enthalten: einen neuen Record, die Impfregeln, den Anzeigetext, die Zuordnung der
Portal-Codes, die JSON-Registrierung und "den Code nach jedem Switch über Impfstoffe durchsuchen".

Mach dann die Schritte unten und vergleich mit dem Whiteboard:

- **Der Compiler** findet zwei Stellen: den Anzeigetext und die Regeln.
- **JSON** braucht überhaupt keine Änderung.
- **Die Zuordnung der Portal-Codes** finden nur die Menschen, weil der Compiler keine Strings
  prüfen kann. Diskutiert, ob es sich lohnt, auch das prüfbar zu machen. Zum Beispiel würde ein
  vollständiger Switch von `Vaccine` auf den Portal-Code einen fehlenden Code zum Compilerfehler
  machen. Bei drei Impfstoffen lohnt es sich vielleicht nicht, bei dreißig schon.

### Code

Zuerst `rm data/appointments.json`. Schritt 4 hat Maria bereits gebucht (nur Grippe), ohne Reset
wäre ihre Anfrage also nur ein Duplikat.

1. Füg den Fall in `Domain.cs` hinzu:

   ```csharp
   public sealed record Rsv : Vaccine;
   ```

2. Bauen. Der Compiler liefert die To-do-Liste:

   ```text
   Domain.cs(22,54):      error CS8509: ... the pattern 'Vaccination.Rsv' is not covered.
   Eligibility.cs(31,23): error CS8509: ... the pattern 'Vaccination.Rsv' is not covered.
   ```

3. Korrigier `Vaccine.ToString()`:

   ```csharp
   Rsv => "RSV",
   ```

4. Korrigier `Rules.Check` mit der RSV-Regel (eine Dosis, ab 75, oder ab 60 mit Risikofaktoren).
   Die Arme kommen in den `wanted switch`, nach `Covid c => c,`:

   ```csharp
   Rsv when LastDose<Rsv>(past) is not null
       => new NotRecommended("RSV is a single dose, already vaccinated"),
   Rsv when age < 60 || (age < 75 && !person.RiskFactors)
       => new NotRecommended("RSV: from 75, or from 60 with risk factors"),
   Rsv r
       => r,
   ```

5. Ausführen. Maria bekommt immer noch "'rsv' is not offered here". Die Zuordnung der Portal-Codes
   am Anfang von `Rules.Check` ist ein `string`-Switch mit Fallback, und der Compiler kann keine
   Strings prüfen. Füg die Zeile nach `"covid" => new Covid(),` ein:

   ```csharp
   "rsv" => new Rsv(),
   ```

6. Noch einmal `rm data/appointments.json` (der Lauf in 5. hat Maria ohne RSV gebucht), dann erneut
   ausführen.

**Ausführen.** Ausschnitt:

```text
006-steiner.json
  Maria Steiner    rsv    yes: RSV
  Maria Steiner    flu    yes: Influenza (high dose)
  => booked Wed 25 Nov 08:15, booth 1
```

**Was wir nicht angefasst haben:** JSON. Dank `closed` und `InferClosedTypePolymorphism` wird RSV
ohne weitere Änderungen mit `"$type": "Rsv"` gespeichert und geladen. Öffne `data/appointments.json`
und such nach `Rsv`, um es zu zeigen. In C# 14 würdest du eine `[JsonDerivedType]`-Liste an der
Basisklasse pflegen. Vergisst du dort `Rsv`, kompiliert alles problemlos und wirft dann zur
Laufzeit eine `NotSupportedException`, sobald zum ersten Mal ein RSV-Termin gespeichert wird
(`14_JsonUnions`, `CSharp14Way.cs`).

---

## Abschließende Modellierungsübung: der Impftag (ohne Code)

Diese Übung ist nur zum Diskutieren. Das Beispiel setzt sie nicht um, die Gruppe entwirft sie also
von Grund auf.

> **Anforderungen**
>
> - Am Impftag durchläuft ein Termin mehrere Zustände: gebucht, eingecheckt (mit Uhrzeit), geimpft
>   (mit Uhrzeit und einer Chargennummer pro Impfstoff), verschoben (mit Grund, zum Beispiel Fieber;
>   die Person muss neu buchen) oder nicht erschienen.
> - Der Status wird in `appointments.json` gespeichert.
> - Auswertungen: nicht erschienene Personen pro Tag, und alle verwendeten Chargennummern.

**Frag:**

1. Union oder geschlossene Hierarchie?
2. Gehört der Status zum Termin oder zu jedem einzelnen Patienten? Ein Familienmitglied mit Fieber
   kann nicht geimpft werden, die anderen schon.
3. Wie speichern wir "eine Chargennummer pro Impfstoff"?
4. Wie sieht ein Statuswechsel im Code aus?

**Antworten, die du wahrscheinlich hörst, und wie du reagierst:**

- *"Ein Enum `Status` plus nullable Felder `CheckedInAt`, `LotNumbers` und `DeferralReason`."* Die
  klassische Mischung aus nullable Feldern, die `04_DomainModeling` ablöst: Die Typen sagen nicht,
  welche Felder zu welchem Status gehören.
- *"Eine Union aus Status-Records."* Im Speicher in Ordnung, aber der Status wird gespeichert. Eine
  Union schreibt den reinen Wert ohne Discriminator, und `Booked` und `NoShow` haben überhaupt keine
  Properties. System.Text.Json weigert sich, so eine Union mit dem Structural Classifier
  einzurichten: "the case type ... can never be selected uniquely".

**Eine mögliche Antwort.** Es ist dasselbe Muster wie bei `Vaccine`: Uns gehören die Typen und wir
speichern sie, also eine geschlossene Hierarchie mit abgeleiteter Polymorphie, plus eine Property
`AppointmentStatus Status` im Record, der den Status hält. Das kompiliert und übersteht den Weg
durch JSON und zurück:

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

Das ist ein Ausgangspunkt, nicht die Lösung. Die Fragen 2 und 3 sollten Löcher hineinbohren:
`Status` gehört wahrscheinlich zu `Patient`, und ein `string[]` mit Chargennummern passt nur über
die Position zu den Impfstoffen. Für eine Version mit Code: Schreib die Übergänge als Switch über ein
Tupel `(AppointmentStatus, Event)`.

---

## Übungen für die Teilnehmenden

1. **Bring `NoFreeSlot` zum Vorschein.** Setz in `Booking.cs` `Closes = new(8, 30)` und
   `BookingDays = 1`, dann `rm data/appointments.json` und ausführen. Die Familie Huber bekommt "no
   2 free slots in a row".
2. **Füg einen vierten Impfstoff hinzu**, zum Beispiel Pneumokokken (ab 60, eine Dosis). Zähl, zu
   wie vielen Stellen dich der Compiler schickt und wie viele er nicht finden kann (die Zuordnung
   der Portal-Codes).
3. **Zeig die Impfberechtigung aller Familienmitglieder nebeneinander** und verwende einen Switch
   über ein Tupel `(Eligibility, Eligibility)` für "Grippe und COVID-19 im selben Termin".
4. **Mach es durchgehend strikt:** Was passiert, wenn eine Inbox-Datei sowohl `person` als auch
   `members` enthält? Probier es aus und entscheide dann, ob die App die Datei ablehnen soll.
