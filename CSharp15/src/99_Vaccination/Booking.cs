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
