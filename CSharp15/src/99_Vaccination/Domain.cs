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
        Rsv => "RSV",
    };
}

public sealed record Influenza(bool HighDose) : Vaccine;
public sealed record Covid : Vaccine;
public sealed record Rsv : Vaccine;

// One entry in a person's vaccination record (imported from the e-vaccination pass).
public sealed record VaccinationRecord(string InsuranceNumber, Vaccine Vaccine, DateOnly Date);

// A family is vaccinated one after another in the same booth: an appointment
// starts at `Start` and takes one slot per patient.
public readonly record struct Slot(DateOnly Date, TimeOnly Time, int Booth);
public sealed record Patient(Person Person, Vaccine[] Vaccines);
public sealed record Appointment(string Email, Slot Start, Patient[] Patients);
