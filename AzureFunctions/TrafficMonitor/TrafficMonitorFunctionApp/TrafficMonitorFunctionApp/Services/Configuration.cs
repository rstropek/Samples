using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TrafficMonitor.Services
{
    public class Configuration
    {
        public Configuration()
        {
            var builder = new ConfigurationBuilder().AddEnvironmentVariables("SECCTRL_");
            var configuration = builder.Build();

            Host = configuration["HOST"];
            AuthKey = configuration["AUTH_KEY"];
            NumberOfAustrianCars = Convert.ToInt32(configuration["NUMBER_CARS_A"]);
            NumberOfGermanCars = Convert.ToInt32(configuration["NUMBER_CARS_D"]);
            NumberOfAustrianSectionControls = Convert.ToInt32(configuration["NUMBER_SECTION_CONTROLS_A"]);
            NumberOfGermanSectionControls = Convert.ToInt32(configuration["NUMBER_SECTION_CONTROLS_D"]);
            OpenAlprKey = configuration["OPENALPR_KEY"];
        }

        public string Host { get; }
        public string AuthKey { get; }
        public int NumberOfAustrianCars { get; }
        public int NumberOfGermanCars { get; }
        public int NumberOfAustrianSectionControls { get; }
        public int NumberOfGermanSectionControls { get; }
        public string OpenAlprKey { get; }
    }
}
