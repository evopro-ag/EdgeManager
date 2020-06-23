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
        public static ILog GetLogger()
        {
            return LogManager.GetLogger("EdgeManager", "Logger");
        }

        public static ILog GetLogger(Type type)
        {
            return LogManager.GetLogger("EdgeManager", type);
        }
    }
}
