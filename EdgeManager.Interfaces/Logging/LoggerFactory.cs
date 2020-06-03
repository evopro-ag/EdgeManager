using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace EdgeManager.Interfaces.Logging
{
    public static class LoggerFactory
    {
        public static ILog GetLogger<T>()
        {
            return GetTypedLogger(typeof(T));
        }

        public static ILog GetTypedLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }
    }
}
