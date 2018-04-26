using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrafficMonitor.Model
{
    public class Enter
    {
        [JsonProperty(PropertyName = "cid")]
        public string StartCameraID { get; set; }

        [JsonProperty(PropertyName = "ts")]
        public long Timestamp { get; set; }
    }
}
