using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrafficMonitor.Model
{
    /// <summary>
    /// Represents a car
    /// </summary>
    public class Car
    {
        /// <summary>
        /// ID of the car
        /// </summary>
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }

        /// <summary>
        /// Nationality of the car
        /// </summary>
        /// <remarks>
        /// In this sample, we work with "eu-at" for Austria and "eu-de" for Germany.
        /// </remarks>
        [JsonProperty(PropertyName = "nat")]
        public string Nationality { get; set; }

        /// <summary>
        /// License plate without and special characters
        /// </summary>
        [JsonProperty(PropertyName = "lp")]
        public string LicensePlate { get; set; }

        /// <summary>
        /// Vehicle category
        /// </summary>
        [JsonProperty(PropertyName = "cat", NullValueHandling = NullValueHandling.Ignore)]
        public string VehicleCategory { get; set; }

        /// <summary>
        /// Highway vignettes that the car is assigned
        /// </summary>
        /// <remarks>
        /// Only cars with an  active vignette are allowed to travel on highways.
        /// </remarks>
        [JsonProperty(PropertyName = "vig")]
        public List<Vignette> Vignettes { get; set; } = new List<Vignette>();

        /// <summary>
        /// Section that the car is currently driving through
        /// </summary>
        /// <remarks>
        /// A car enters a section when it is recognized by the start camera of a section control.
        /// A car can be in either in a section or in no section. A car can never be in multiple
        /// sections at the same time.
        /// </remarks>
        [JsonProperty(PropertyName = "sec", NullValueHandling = NullValueHandling.Ignore)]
        public Enter ActiveSection { get; set; }

        /// <summary>
        /// List of violations of the car
        /// </summary>
        /// <remarks>
        /// Examples for violations would be driving without vignette, exceeding speed limit,
        /// ending a section without having started it, etc.
        /// </remarks>
        [JsonProperty(PropertyName = "vio")]
        public List<Violation> Violations { get; set; } = new List<Violation>();
    }
}
