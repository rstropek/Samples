using System;

namespace TrafficMonitor.Services
{
    public class VehicleCategoryGenerator
    {
        private static Random Random = new Random();

        public string GetRandomVehicleClass()
        {
            switch (Random.Next(0, 100))
            {
                case var x when x < 10:
                    return "L";
                case var x when x < 70:
                    return "M";
                case var x when x < 95:
                    return "N";
                default:
                    return "T";
            }
        }
    }
}
