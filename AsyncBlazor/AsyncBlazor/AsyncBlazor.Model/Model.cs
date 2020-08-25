namespace AsyncBlazor.Model
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public class Order
    {
        public string CustomerID { get; set; } = string.Empty;
        public string Product { get; set; } = string.Empty;
        public int Amount { get; set; }
        public string? UserID { get; set; }
    }
}
