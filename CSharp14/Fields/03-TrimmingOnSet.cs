namespace Fields;

public class Product
{
    public required string Name
    {
        get;
        set => field = value.Trim().ToUpperInvariant();
    }
    
    public required decimal Price
    {
        get;
        set => field = Math.Round(value, 2); // Always round to 2 decimal places
    }
}