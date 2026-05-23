using System.Collections.ObjectModel;

namespace Fields;

public static class LinqExtensions
{
    extension(IEnumerable<float> source) { public float AverageOrDefault() => source.DefaultIfEmpty(default(float)).Average(); }
    extension(IEnumerable<double> source) { public double AverageOrDefault() => source.DefaultIfEmpty(default(double)).Average(); }
    extension(IEnumerable<decimal> source) { public decimal AverageOrDefault() => source.DefaultIfEmpty(default(decimal)).Average(); }
    extension(IEnumerable<int> source) { public double AverageOrDefault() => source.DefaultIfEmpty(default(int)).Average(); }
    extension(IEnumerable<long> source) { public double AverageOrDefault() => source.DefaultIfEmpty(default(long)).Average(); }
}

public class DataProcessor
{
    private bool isDirty = true;

    public required ReadOnlyCollection<int> RawData { get; set; }

    public double Average
    {
        get => isDirty ? field = ComputeAverage() : field;
    }

    private double ComputeAverage()
    {
        isDirty = false;
        Console.WriteLine("Computing average");
        return RawData.AverageOrDefault();
    }
}