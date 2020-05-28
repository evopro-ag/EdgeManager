using System;
using System.Linq;
using EdgeManager.Interfaces.Enums;

namespace EdgeManager.Interfaces.Models
{
    public class IoTModuleIdentityInfo
    {
        public string ModuleId { get; set; }
        public ConnectionState ConnectionState { get; set; }
    }
}
