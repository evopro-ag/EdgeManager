using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdgeManager.Gui.Models
{
    public class IotEdgeEventModel
    {
        public string Module { get; set; }
        public string Origin { get; set; }
        public string Interface { get; set; }
        public string Component { get; set; }
        public JObject Payload { get; set; }
        
        [JsonIgnore]
        public string FormattedPayload => JsonConvert.SerializeObject(Payload, Formatting.Indented);

        public DateTime Time { get; set; }

        public IotEdgeEventModel()
        {
            Time = DateTime.Now;
        }
    }
}