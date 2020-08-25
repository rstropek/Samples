using System;
using System.Text.Json.Serialization;

namespace AsyncBlazor.Model
{
    /// <summary>
    /// Represents an order
    /// </summary>
    public class Order
    {
        [JsonPropertyName("orderID")]
        public Guid? OrderID { get; set; }

        [JsonPropertyName("customerID")]
        public string CustomerID { get; set; } = string.Empty;

        [JsonPropertyName("product")]
        public string Product { get; set; } = string.Empty;

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("userID")]
        public string? UserID { get; set; }
    }
}
