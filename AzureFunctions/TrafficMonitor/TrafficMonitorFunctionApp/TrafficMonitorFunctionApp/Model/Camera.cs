using Newtonsoft.Json;

namespace TrafficMonitor.Model
{
    /// <summary>
    /// Represents a traffic camera
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// ID of the camera
        /// </summary>
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }

        /// <summary>
        /// GPS location of the camera in the format "<lat>;</long>"
        /// </summary>
        /// <remarks>
        /// Contains dummy value in this sample
        /// </remarks>
        [JsonProperty(PropertyName = "loc")]
        public string Location { get; set; }

        /// <summary>
        /// Additional properties relevant only for cameras at the start of a section control
        /// </summary>
        /// <remarks>
        /// This property is null if the camera is not starting a section control. In real world,
        /// a section control might have multiple cameras at it start. However, in this sample we
        /// simplify. There is only one camera at the start.
        /// </remarks>
        [JsonProperty(PropertyName = "start", NullValueHandling = NullValueHandling.Ignore)]
        public StartCamera Start { get; set; }

        /// <summary>
        /// Additional properties relevant only for cameras at the end of a section control
        /// </summary>
        /// <remarks>
        /// This property is null if the camera is not ending a section control. In real world,
        /// a section control might have multiple cameras at it end. However, in this sample we
        /// simplify. There is only one camera at the end.
        /// </remarks>
        [JsonProperty(PropertyName = "end", NullValueHandling = NullValueHandling.Ignore)]
        public EndCamera End { get; set; }
    }

    /// <summary>
    /// Additional properties for cameras starting a section control
    /// </summary>
    public class StartCamera
    {
        /// <summary>
        /// Camera ID that ends the section control
        /// </summary>
        /// <remarks>
        /// In real world, a section control might have multiple cameras at it start and end. However, 
        /// in this sample we simplify. There is only one camera at the start and at the end.
        /// </remarks>
        [JsonProperty(PropertyName = "eid")]
        public string EndCameraID { get; set; }
    }

    /// <summary>
    /// Additional properties for cameras ending a section control
    /// </summary>
    public class EndCamera
    {
        /// <summary>
        /// Camera ID that starts the section control
        /// </summary>
        /// <remarks>
        /// In real world, a section control might have multiple cameras at it start and end. However, 
        /// in this sample we simplify. There is only one camera at the start and at the end.
        /// </remarks>
        [JsonProperty(PropertyName = "sid")]
        public string StartCameraID { get; set; }

        /// <summary>
        /// Distance from start to end of section control in meters
        /// </summary>
        [JsonProperty(PropertyName = "dist")]
        public int DistanceFromStart { get; set; }

        /// <summary>
        /// Maximum average speed in km/h
        /// </summary>
        [JsonProperty(PropertyName = "vmax")]
        public int MaximumAverageSpeed { get; set; }
    }
}
