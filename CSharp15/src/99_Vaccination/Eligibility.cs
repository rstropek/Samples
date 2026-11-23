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
            "rsv" => new Rsv(),
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

            Rsv when LastDose<Rsv>(past) is not null
                => new NotRecommended("RSV is a single dose, already vaccinated"),
            Rsv when age < 60 || (age < 75 && !person.RiskFactors)
                => new NotRecommended("RSV: from 75, or from 60 with risk factors"),
            Rsv r
                => r,
        };
    }

    private static DateOnly? LastDose<T>(IEnumerable<VaccinationRecord> past) where T : Vaccine =>
        past.Where(r => r.Vaccine is T).Max(r => (DateOnly?)r.Date);
}
