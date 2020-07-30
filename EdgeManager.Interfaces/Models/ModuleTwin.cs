using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace EdgeManager.Interfaces.Models
{
    public class ModuleTwin
    {
        public string ModuleId { get; set; }
        public ModuleProperties Properties { get; set; }
    }

    public class ModuleProperties
    {
        public JObject Desired { get; set; }
        public JObject Reported { get; set; }
    }
}
