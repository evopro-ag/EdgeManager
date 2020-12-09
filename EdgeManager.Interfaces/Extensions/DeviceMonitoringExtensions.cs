using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EdgeManager.Interfaces.Logging;
using EdgeManager.Interfaces.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdgeManager.Interfaces.Extensions
{
    public static class DeviceMonitoringExtensions
    {
        public static IObservable<string> GetTelemetryMessages(this ICommandHandler commandHandler)
        {
            return commandHandler.OutputLines
                    .Where(s => !string.IsNullOrEmpty(s))
                    .GroupByUntil(s => s.Equals("event:"), 
                        g => commandHandler.OutputLines.Where(e => e.Equals("event:")))
                    .Where(g => !g.Key)
                    .Select(g => g.ToArray())
                    .Switch()
                    .Select(l => string.Join("\n", l))
                ;
        }
        
        public static IObservable<JObject> GetTelemetryMessagesInJsonFormat(this ICommandHandler commandHandler)
        {
            return commandHandler.OutputLines
                    .Where(s => !string.IsNullOrEmpty(s))
                    .GroupByUntil(s => s.Equals("{"), 
                        g => commandHandler.OutputLines.Where(e => e.Equals("}")))
                    .Select(g => g.ToArray())
                    .Switch()
                    .Select(l => "{"+string.Join("\n", l))
                    .Select(e =>
                    {
                        try
                        {
                            return JsonConvert.DeserializeObject<JObject>(e);
                        }
                        catch (Exception ex)
                        {
                            LoggerFactory.GetLogger().Error("error while parsing json message", ex);
                        }

                        return null;
                    })
                    .Where(o => o != null)
                ;
        }

    }
}