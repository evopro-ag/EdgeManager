using System;
using System.Linq;

namespace EdgeManager.Interfaces.Models
{
    public class IoTHubInfo
    {
        //[JsonProperty("name")] // see details here: https://www.newtonsoft.com/json/help/html/JsonPropertyName.htm
        public string Name { get; set; }
        public IoTHubProperties Properties { get; set; }
    }
}
