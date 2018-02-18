using System;

namespace RentalManagement.Services
{
    public class Rating : IRating
    {
        public decimal CalculateTotalCosts(TimeSpan rentDuration)
        {
            const decimal firstHour = 3m;

            if (rentDuration.TotalMinutes <= 15d)
            {
                // Under 15 minutes -> free
                return 0m;
            }
            else if (rentDuration.TotalHours <= 1d)
            {
                // Up to one hour flat fee of 3€
                return firstHour;
            }

            // Flat fee for first hour + 5€ per additional started hour
            return firstHour + (decimal)Math.Ceiling(rentDuration.Add(TimeSpan.FromHours(-1)).TotalHours) * 5m;
        }

    }
}
