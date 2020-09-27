using Newtonsoft.Json;

namespace HotlineKatalog.WebSockets.Models
{
    public class WebSocketEventResponseModel
    {
        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
