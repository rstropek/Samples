using System;

namespace RentalManagement.Model
{
    public class RentalEndResult : RentalEnd
    {
        public DateTime BeginOfRental { get; set; }
        public DateTime EndOfRental { get; set; }
        public decimal TotalCosts { get; set; }
    }
}
