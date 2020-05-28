using System;
using System.Linq;

namespace EdgeManager.Interfaces.Models
{
    public class IoTDirectMethodReply
    {
        public object Payload { get; set; }
        public int Status { get; set; }
    }
}
