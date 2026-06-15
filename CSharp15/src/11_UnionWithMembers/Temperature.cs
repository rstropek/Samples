// ============================================================================
//  Alternative: declare the union `partial` and put the members in another
//  file/partial. Useful to keep a generated/declaration part separate from
//  hand-written logic.
// ============================================================================
namespace UnionMembers;

public record class Celsius(double Degrees);
public record class Fahrenheit(double Degrees);

public partial union Temperature(Celsius, Fahrenheit);

public partial union Temperature
{
    public double AsCelsius => this switch
    {
        Celsius c    => c.Degrees,
        Fahrenheit f => (f.Degrees - 32) * 5.0 / 9.0,
        null         => double.NaN,
    };

    public override string ToString() => $"{AsCelsius:F1} °C";
}
