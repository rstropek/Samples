using System.Runtime.CompilerServices;

var x = 5;
Verify.Lower(x * 2, Convert.ToInt32(Math.Floor(Math.PI)));

public static class Verify
{
    public static void Lower(int argument, int maxValue,
        [CallerArgumentExpression(nameof(argument))] string? argumentExpression = null,
        [CallerArgumentExpression(nameof(maxValue))] string? maxValueExpression = null)
    {
        if (argument > maxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(argument),
                $"{argumentExpression} must be lower or equal {maxValueExpression}");
        }
    }
}
