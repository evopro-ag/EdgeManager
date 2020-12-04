using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using EdgeManager.Interfaces.Services;

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

    }
}