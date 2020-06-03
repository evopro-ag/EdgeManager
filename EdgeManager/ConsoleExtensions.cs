using System;
using System.Linq;
using Newtonsoft.Json;

namespace EdgeManager
{
    public static class ConsoleExtensions
    {
        public static object Dump(this object o, string description = "")
        {
            Console.WriteLine(description);
            var serializeObject = JsonConvert.SerializeObject(o, Formatting.Indented);
            Console.WriteLine(serializeObject);
            return o;
        }
    }
}