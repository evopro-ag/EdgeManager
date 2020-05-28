using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeManager
{
    public class IoTHubInfo
    {
        //[JsonProperty("name")] // see details here: https://www.newtonsoft.com/json/help/html/JsonPropertyName.htm
        public string Name { get; set; }
        public IoTHubProperties Properties { get; set; }
    }
}
