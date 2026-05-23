namespace Ref;

public static class VariablesAndExpressions
{
    public static void RefVariables()
    {
        // Reference variables are variables that store references to other variables.
        int x = 10;
        ref int refX = ref x;
        refX = 20;
        Console.WriteLine(x); // 20
    }

    public static void RefExpressions()
    {
        int[] values = [1, 2, 3, 4, 5];
        ref int value = ref values[2];
        value = 10;
        Console.WriteLine(values[2]); // 10

        ref int value2 = ref value;

        DateOnly date1 = new(2021, 1, 1);
        DateOnly date2 = new(2021, 1, 2);

        ref DateOnly earliestDate = ref (date1 < date2 ? ref date1 : ref date2);

        earliestDate = earliestDate.AddYears(1);
        Console.WriteLine(date1); // 2022-01-01
    }
}

