using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeManager
{
    class RestartModulePayload : DirectMethodPayloadBase
    {
        [JsonProperty("schemaVersion")] public string SchemaVersion { get; set; } = "1.0";
        [JsonProperty("id")] public string ModuleId { get; set; } = "Marcolino";
    }
}
