using System.Collections.Generic;
using EdgeManager.Interfaces.Models;

namespace EdgeManager.Interfaces.Settings
{
    public class ApplicationSettings
    {
        public Dictionary<string, JsonCommand> CommandCache { get; set; } = new Dictionary<string, JsonCommand>();
        public AzureAccountInfo AzureAccountInfo { get; set; }
        public IoTHubInfo LastUsedIoTHub { get; set; }
        public IoTHubInfo[] LastCheckedIoTHubs { get; set; }
    }
}