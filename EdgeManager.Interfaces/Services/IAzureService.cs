using System;
using System.Linq;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Models;

namespace EdgeManager.Interfaces.Services
{
    public interface IAzureService
    {
        IObservable<JsonCommand> JsonCommands {get;}
        Task<IoTHubInfo[]> GetIoTHubs(bool reload = false);
        Task<IoTDeviceInfo[]> GetIoTDevices(string hubName, bool reload = false);
        Task<IoTModuleIdentityInfo[]> GetIoTModules(string hubName, string deviceId, bool reload = false);
        Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload);

    }
}