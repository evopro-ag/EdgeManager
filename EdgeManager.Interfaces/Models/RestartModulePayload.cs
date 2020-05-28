using System;
using System.Linq;
using Newtonsoft.Json;

namespace EdgeManager.Interfaces.Models
{
    class RestartModulePayload : DirectMethodPayloadBase
    {
        [JsonProperty("schemaVersion")] public string SchemaVersion { get; set; } = "1.0";
        [JsonProperty("id")] public string ModuleId { get; set; } = "Marcolino";
    }
}
