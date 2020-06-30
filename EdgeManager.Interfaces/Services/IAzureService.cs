using System;
using System.Linq;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Models;

namespace EdgeManager.Interfaces.Services
{
    public interface IAzureService
    {
        IObservable<JsonCommand> JsonCommands {get;}
        Task<IoTHubInfo[]> GetIoTHubs();
        Task<IoTDeviceInfo[]> GetIoTDevices(string hubName);
        Task<IoTModuleIdentityInfo[]> GetIoTModules(string hubName, string deviceId);
        Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload);

    }
}