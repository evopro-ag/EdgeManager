using System;
using System.Linq;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Models;

namespace EdgeManager.Interfaces.Services
{
    public interface IAzureService
    {
        Task<IoTHubInfo[]> GetIoTHubs();
        Task<IoTDeviceInfo[]> GetIoTDevices(string hubName);
        Task<IoTModuleIdentityInfo[]> GetIoTModule(string hubName, string deviceId);
        Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload);
    }
}