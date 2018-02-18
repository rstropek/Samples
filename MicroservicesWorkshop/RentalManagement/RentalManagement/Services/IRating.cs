using System;

namespace RentalManagement.Services
{
    public interface IRating
    {
        decimal CalculateTotalCosts(TimeSpan rentDuration);
    }
}
