using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EdgeManager.Interfaces.Models;
using EdgeManager.Interfaces.Services;

namespace EdgeManager.Gui.Design
{
    class DesignAzureService : IAzureService
    {
        public Task<IoTHubInfo[]> GetIoTHubs()
        {
            return Task.FromResult(new[]
            {
                new IoTHubInfo {Name = "Hub1", Properties = new IoTHubProperties()},
                new IoTHubInfo {Name = "Hub2", Properties = new IoTHubProperties()},
                new IoTHubInfo {Name = "Hub3", Properties = new IoTHubProperties()}
            });
        }

        public Task<IoTDeviceInfo[]> GetIoTDevices(string hubName)
        {
            throw new NotImplementedException();
        }

        public Task<IoTModuleIdentityInfo[]> GetIoTModule(string hubName, string deviceId)
        {
            throw new NotImplementedException();
        }

        public Task<IoTDirectMethodReply> CallMethod(string method, string hubName, string deviceId, string moduleId, DirectMethodPayloadBase payload)
        {
            throw new NotImplementedException();
        }
    }
}
