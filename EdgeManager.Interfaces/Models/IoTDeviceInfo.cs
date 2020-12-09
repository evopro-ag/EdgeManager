using System;
using System.Linq;
using EdgeManager.Interfaces.Enums;

namespace EdgeManager.Interfaces.Models
{
    public class IoTDeviceInfo
    {
        public string DeviceId { get; set; }
        public string HubName { get; set; }
        public ConnectionState ConnectionState { get; set; }
        public Capabilities Capabilities { get; set; }
    }
}
