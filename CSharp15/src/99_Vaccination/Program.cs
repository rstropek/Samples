// ============================================================================
//  End-to-end sample: a community vaccination center (flu, COVID-19, RSV).
//
//  1. Reads booking requests that the city's portal drops into data/inbox/.
//  2. Checks each requested vaccine against the person's vaccination history.
//  3. Books back-to-back slots (a family is vaccinated one after another).
//  4. Saves all appointments to data/appointments.json and prints the plan.
//
//  Run it from this folder:  dotnet run
//  Start over:               rm data/appointments.json
// ============================================================================
using System.Text.Json;
using Vaccination;

// A fixed "today" (the workshop day), so the output is the same on every run.
DateOnly today = new(2026, 11, 24);

string appointmentsFile = Path.Combine("data", "appointments.json");
if (!File.Exists(appointmentsFile)) File.Copy(Path.Combine("data", "appointments.seed.json"), appointmentsFile);

var history = Storage.Read<List<VaccinationRecord>>(Path.Combine("data", "history.json"));
var appointments = Storage.Read<List<Appointment>>(appointmentsFile);
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

Storage.Write(appointmentsFile, appointments);

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
