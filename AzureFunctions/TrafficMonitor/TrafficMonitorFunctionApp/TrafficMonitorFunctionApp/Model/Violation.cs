using Newtonsoft.Json;

namespace TrafficMonitor.Model
{
    public class Violation
    {
        public Violation(long timestamp, string description)
        {
            Timestamp = timestamp;
            Description = description;
        }

        [JsonProperty(PropertyName = "ts")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "desc")]
        public string Description { get; set; }
    }
}
