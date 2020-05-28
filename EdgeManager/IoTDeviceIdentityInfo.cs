using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeManager
{
    public class IoTDeviceIdentityInfo
    {
        public string DeviceId { get; set; }
        public ConnectionState ConnectionState { get; set; }
        public Capabilities Capabilities { get; set; }
    }
}
