using Newtonsoft.Json;
using System.Collections.Generic;

namespace TrafficMonitor.Model
{
    public class EmptyPlateRead
    {
        [JsonProperty(PropertyName = "ts")]
        public long ReadTimestamp { get; set; }

        [JsonProperty(PropertyName = "cid")]
        public string CameraID { get; set; }

        [JsonProperty(PropertyName = "iid")]
        public string ImageID { get; set; }
    }
    
    public class PlateRead : EmptyPlateRead
    {
        [JsonProperty(PropertyName = "lp")]
        public string LicensePlate { get; set; }

        [JsonProperty(PropertyName = "conf")]
        public double Confidence { get; set; }

        [JsonProperty(PropertyName = "nat")]
        public string Nationality { get; set; }

        [JsonProperty(PropertyName = "nconf")]
        public double NationalityConfidence { get; set; }

        [JsonProperty(PropertyName = "carid", NullValueHandling = NullValueHandling.Ignore)]
        public string CarID { get; set; }
    }
}
