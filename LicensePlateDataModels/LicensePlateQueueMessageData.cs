using Newtonsoft.Json;

namespace LicensePlateDataModels
{
    public class LicensePlateQueueMessageData
    {
        [JsonProperty(PropertyName = "fileName")]
        public string FileName { get; set; }

        [JsonProperty(PropertyName = "licensePlateText")]
        public string LicensePlateText { get; set; }

        [JsonProperty(PropertyName = "timeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonProperty(PropertyName = "exported")]
        public bool Exported { get; set; }
    }

}
