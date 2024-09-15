namespace Ref;

public static class ReturnValues
{
    static ref DateOnly FindEarliestDate(/*IReadOnlyList<DateOnly>*/ DateOnly[] dates)
    {
        DateOnly earliestDate = dates[0];
        var earliestIndex = 0;
        for (int i = 1; i < dates.Length /* Count */; i++)
        {
            if (dates[i] < earliestDate)
            {
                earliestDate = dates[i];
                earliestIndex = i;
            }
        }

        // Note that we are returning a reference(!) to an element of the array.
        return ref dates[earliestIndex];

        // Question: Why doesn't this compile if we use IReadOnlyList<DateOnly> instead of DateOnly[]?
        #region Answer
        // The index accessor of IReadOnlyList<T> returns a value, not a reference.
        #endregion
    }

    public static void RefReturns()
    {
        DateOnly[] dates = [new(2021, 1, 1), new(2021, 1, 2), new(2021, 1, 3), new(2021, 1, 4), new(2021, 1, 5),];

        // Note that we need to use a reference variable to store the reference to the earliest date.
        ref DateOnly earliestDate = ref FindEarliestDate(dates);
        earliestDate = new DateOnly(2021, 1, 6);

        foreach (var date in dates)
        {
            Console.WriteLine(date);
        }
    }
}
