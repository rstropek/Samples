// ============================================================================
//  Domain modelling with unions: "make illegal states unrepresentable".
//  Each state carries exactly the data that is valid in that state — something
//  a single class with nullable fields cannot express cleanly.
// ============================================================================
namespace Domain;

// States of an order. Note how the *data differs per state*:
public record class Draft(IReadOnlyList<string> Items);
public record class Placed(IReadOnlyList<string> Items, DateOnly OrderedOn);
public record class Shipped(string TrackingNumber, DateOnly ShippedOn);
public record class Delivered(DateOnly DeliveredOn);
public record class Cancelled(string Reason);

public union OrderState(Draft, Placed, Shipped, Delivered, Cancelled);

internal static class Program
{
    // A transition function. The exhaustive switch guarantees every state is
    // considered whenever the state set grows.
    public static OrderState Advance(OrderState state) => state switch
    {
        Draft { Items.Count: 0 }  => new Cancelled("Cannot place an empty order"),
        Draft d                   => new Placed(d.Items, DateOnly.FromDateTime(DateTime.Now)),
        Placed p                  => new Shipped($"TRK-{p.OrderedOn:yyyyMMdd}-001", p.OrderedOn.AddDays(1)),
        Shipped s                 => new Delivered(s.ShippedOn.AddDays(2)),
        Delivered done            => done,        // terminal
        Cancelled c               => c,           // terminal
        null                      => throw new InvalidOperationException("uninitialised OrderState"),
    };

    public static string Render(OrderState state) => state switch
    {
        Draft d      => $"Draft with {d.Items.Count} item(s)",
        Placed p     => $"Placed on {p.OrderedOn}",
        Shipped s    => $"Shipped, tracking {s.TrackingNumber}",
        Delivered d  => $"Delivered on {d.DeliveredOn}",
        Cancelled c  => $"Cancelled: {c.Reason}",
        null         => "<uninitialised order>",
    };

    private static void Main()
    {
        OrderState state = new Draft(["Keyboard", "Mouse"]);

        for (int step = 0; step < 5; step++)
        {
            Console.WriteLine($"{step}: {Render(state)}");
            var next = Advance(state);
            if (ReferenceEquals(next.Value, state.Value)) break; // reached terminal
            state = next;
        }
    }
}
